using Microsoft.EntityFrameworkCore;
using ContosoDashboard.Data;
using ContosoDashboard.Models;

namespace ContosoDashboard.Services;

public class DocumentService : IDocumentService
{
    private readonly ApplicationDbContext _context;
    private readonly IFileStorageService _fileStorageService;
    private readonly INotificationService _notificationService;

    public DocumentService(ApplicationDbContext context, IFileStorageService fileStorageService, INotificationService notificationService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
        _notificationService = notificationService;
    }

    public async Task<List<Document>> GetUserDocumentsAsync(int userId, string? search = null, int? projectId = null, string? category = null)
    {
        var query = _context.Documents
            .Where(d => d.UploadedByUserId == userId && !d.IsDeleted)
            .Include(d => d.UploadedByUser)
            .Include(d => d.Project)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(d =>
                d.Title.Contains(term) ||
                (d.Description != null && d.Description.Contains(term)) ||
                (d.Tags != null && d.Tags.Contains(term)) ||
                d.Project != null && d.Project.Name.Contains(term) ||
                d.UploadedByUser.DisplayName.Contains(term));
        }

        if (projectId.HasValue)
        {
            query = query.Where(d => d.ProjectId == projectId.Value);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(d => d.Category == category);
        }

        return await query
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync();
    }

    public async Task<List<Document>> GetProjectDocumentsAsync(int projectId, int requestingUserId)
    {
        var project = await _context.Projects
            .Include(p => p.ProjectMembers)
            .FirstOrDefaultAsync(p => p.ProjectId == projectId);

        if (project == null)
        {
            return new List<Document>();
        }

        var isAuthorized = project.ProjectManagerId == requestingUserId ||
                           project.ProjectMembers.Any(pm => pm.UserId == requestingUserId);

        if (!isAuthorized)
        {
            return new List<Document>();
        }

        return await _context.Documents
            .Include(d => d.UploadedByUser)
            .Include(d => d.Project)
            .Where(d => d.ProjectId == projectId && !d.IsDeleted)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync();
    }

    public async Task<List<Document>> GetRecentUserDocumentsAsync(int userId, int count = 5)
    {
        return await _context.Documents
            .Where(d => d.UploadedByUserId == userId && !d.IsDeleted)
            .OrderByDescending(d => d.UploadedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Document?> GetDocumentByIdAsync(int documentId, int requestingUserId)
    {
        var document = await _context.Documents
            .Include(d => d.UploadedByUser)
            .Include(d => d.Project)
            .Include(d => d.SharedWith)
            .FirstOrDefaultAsync(d => d.DocumentId == documentId && !d.IsDeleted);

        if (document == null) return null;

        var isOwner = document.UploadedByUserId == requestingUserId;
        var isProjectManager = document.Project != null && document.Project.ProjectManagerId == requestingUserId;
        var isProjectMember = document.Project != null && document.Project.ProjectMembers.Any(pm => pm.UserId == requestingUserId);
        var isShared = document.SharedWith.Any(s => s.UserId == requestingUserId && s.IsActive);

        if (!isOwner && !isProjectManager && !isProjectMember && !isShared)
        {
            return null;
        }

        return document;
    }

    public async Task<Document> UploadDocumentAsync(DocumentUploadRequest request, int userId)
    {
        if (string.IsNullOrWhiteSpace(request.Title)) throw new InvalidOperationException("Document title is required.");
        if (request.FileStream == null || request.FileSize <= 0) throw new InvalidOperationException("File is required.");

        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(request.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !allowedExtensions.Contains(extension.ToLowerInvariant()))
        {
            throw new InvalidOperationException("Unsupported file type.");
        }

        if (request.FileSize > 25 * 1024 * 1024)
        {
            throw new InvalidOperationException("File exceeds the 25 MB limit.");
        }

        var relativePath = await _fileStorageService.UploadAsync(request.FileStream, request.FileName, request.ContentType, userId, request.ProjectId);
        var document = new Document
        {
            Title = request.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            Category = string.IsNullOrWhiteSpace(request.Category) ? "Other" : request.Category.Trim(),
            FileName = Path.GetFileName(request.FileName),
            FilePath = relativePath,
            FileSize = request.FileSize,
            FileType = string.IsNullOrWhiteSpace(request.ContentType) ? "application/octet-stream" : request.ContentType,
            Tags = request.Tags,
            UploadedByUserId = userId,
            ProjectId = request.ProjectId,
            UploadedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        return document;
    }

    public async Task<bool> DeleteDocumentAsync(int documentId, int requestingUserId)
    {
        var document = await _context.Documents.FirstOrDefaultAsync(d => d.DocumentId == documentId && !d.IsDeleted);
        if (document == null) return false;

        var project = document.ProjectId.HasValue
            ? await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == document.ProjectId.Value)
            : null;

        var isOwner = document.UploadedByUserId == requestingUserId;
        var isProjectManager = project != null && project.ProjectManagerId == requestingUserId;

        if (!isOwner && !isProjectManager)
        {
            return false;
        }

        document.IsDeleted = true;
        document.UpdatedAt = DateTime.UtcNow;

        await _fileStorageService.DeleteAsync(document.FilePath);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ShareDocumentAsync(int documentId, int recipientUserId, int sharerUserId)
    {
        var document = await _context.Documents
            .Include(d => d.Project)
            .FirstOrDefaultAsync(d => d.DocumentId == documentId && !d.IsDeleted);

        if (document == null) return false;

        var isOwner = document.UploadedByUserId == sharerUserId;
        var isProjectManager = document.Project != null && document.Project.ProjectManagerId == sharerUserId;

        if (!isOwner && !isProjectManager) return false;

        var existingShare = await _context.DocumentShares
            .FirstOrDefaultAsync(s => s.DocumentId == documentId && s.UserId == recipientUserId && s.IsActive);
        if (existingShare != null) return true;

        var share = new DocumentShare
        {
            DocumentId = documentId,
            UserId = recipientUserId,
            SharedByUserId = sharerUserId,
            SharedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.DocumentShares.Add(share);
        await _context.SaveChangesAsync();

        await _notificationService.CreateNotificationAsync(new Notification
        {
            UserId = recipientUserId,
            Title = "Document shared",
            Message = $"{document.Title} was shared with you.",
            Type = NotificationType.DocumentShared,
            Priority = NotificationPriority.Important
        });

        return true;
    }

    public async Task<List<Document>> GetSharedDocumentsAsync(int userId)
    {
        return await _context.DocumentShares
            .Where(s => s.UserId == userId && s.IsActive)
            .Include(s => s.Document)
            .Include(s => s.Document.UploadedByUser)
            .Select(s => s.Document)
            .Where(d => !d.IsDeleted)
            .ToListAsync();
    }
}
