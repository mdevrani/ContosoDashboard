using ContosoDashboard.Models;

namespace ContosoDashboard.Services;

public class DocumentUploadRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = "Other";
    public int? ProjectId { get; set; }
    public string? Tags { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public Stream FileStream { get; set; } = Stream.Null;
}

public interface IDocumentService
{
    Task<List<Document>> GetUserDocumentsAsync(int userId, string? search = null, int? projectId = null, string? category = null);
    Task<List<Document>> GetProjectDocumentsAsync(int projectId, int requestingUserId);
    Task<List<Document>> GetRecentUserDocumentsAsync(int userId, int count = 5);
    Task<Document?> GetDocumentByIdAsync(int documentId, int requestingUserId);
    Task<Document> UploadDocumentAsync(DocumentUploadRequest request, int userId);
    Task<bool> DeleteDocumentAsync(int documentId, int requestingUserId);
    Task<bool> ShareDocumentAsync(int documentId, int recipientUserId, int sharerUserId);
    Task<List<Document>> GetSharedDocumentsAsync(int userId);
}
