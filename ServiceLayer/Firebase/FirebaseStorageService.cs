using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

public class FirebaseStorageService
{
    private readonly StorageClient _storageClient;
    private readonly string _bucketName;

    public FirebaseStorageService(string credentialPath, string bucketName)
    {
        var credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile(credentialPath);
        _storageClient = StorageClient.Create(credential);
        _bucketName = bucketName;
    }

    public async Task<string> UploadFileAsync(IFormFile file, string folder = "")
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is null or empty");

        var objectName = $"{folder}/{Guid.NewGuid()}_{file.FileName}";
        using var stream = file.OpenReadStream();

        await _storageClient.UploadObjectAsync(_bucketName, objectName, file.ContentType, stream);
        return $"https://storage.googleapis.com/{_bucketName}/{objectName}";
    }

    public async Task DeleteFileAsync(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl)) return;

        var uri = new Uri(fileUrl);
        var objectName = uri.AbsolutePath.TrimStart('/').Replace(_bucketName + "/", "");

        await _storageClient.DeleteObjectAsync(_bucketName, objectName);
    }
}
