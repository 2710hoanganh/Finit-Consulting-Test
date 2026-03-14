namespace TechnicalTest.Api.Services.Interfaces;

public interface IS3Service
{
    Task<bool> CreateBucketAsync(string bucketName);
    Task<string>UploadFileAsync(Stream fileStream, string fileName, string contentType);
    Task<Stream> DownloadFileAsync(string fileKey);
    Task DeleteFileAsync(string fileKey);
    string GetUri(string fileKey);
}