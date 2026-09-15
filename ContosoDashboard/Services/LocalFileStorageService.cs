namespace ContosoDashboard.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _rootPath;

    public LocalFileStorageService()
    {
        _rootPath = Path.Combine(AppContext.BaseDirectory, "AppData", "uploads");
        Directory.CreateDirectory(_rootPath);
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string? contentType, int userId, int? projectId = null)
    {
        var extension = Path.GetExtension(fileName);
        var uniqueFileName = $"{Guid.NewGuid():N}{extension}";
        var tenantFolder = projectId.HasValue ? projectId.Value.ToString() : "personal";
        var relativeFolder = Path.Combine(userId.ToString(), tenantFolder).Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
        var targetFolder = Path.Combine(_rootPath, relativeFolder);
        Directory.CreateDirectory(targetFolder);

        var targetPath = Path.Combine(targetFolder, uniqueFileName);
        await using var output = File.Create(targetPath);
        fileStream.Position = 0;
        await fileStream.CopyToAsync(output);

        return Path.Combine(relativeFolder, uniqueFileName).Replace('\\', '/');
    }

    public Task DeleteAsync(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return Task.CompletedTask;
        }

        var fullPath = GetAbsolutePath(relativePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    public async Task<Stream> DownloadAsync(string relativePath)
    {
        var fullPath = GetAbsolutePath(relativePath);
        return await Task.FromResult<Stream>(File.OpenRead(fullPath));
    }

    public string GetAbsolutePath(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return _rootPath;
        }

        var normalized = relativePath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        return Path.GetFullPath(Path.Combine(_rootPath, normalized));
    }
}
