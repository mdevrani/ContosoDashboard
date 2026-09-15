namespace ContosoDashboard.Services;

public interface IFileStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string? contentType, int userId, int? projectId = null);
    Task DeleteAsync(string relativePath);
    Task<Stream> DownloadAsync(string relativePath);
    string GetAbsolutePath(string relativePath);
}
