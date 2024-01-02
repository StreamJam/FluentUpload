using System.Text;
using System.Text.Json;

using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;

namespace FluentUploads;

public class UploadService<TMetadata> : IUploadService<TMetadata>
{
    private readonly IAmazonS3 _s3Client;
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public UploadService(IAmazonS3 s3Client, IHttpContextAccessor? httpContextAccessor = null)
    {
        _s3Client = s3Client;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PresignedUpload> CreateUpload(TMetadata metadata, string filename, long maxFileSizeBytes = 1024 * 1024, string? callbackId = null)
    {
        string? displayName = _httpContextAccessor?.HttpContext?.GetEndpoint()?.DisplayName;
        string fileId = Guid.NewGuid().ToString();

        var request = new GetPreSignedUrlRequest()
        {
            BucketName = "fluentuploads-test",
            Key = $"{fileId}-{filename}",
            Expires = DateTime.UtcNow.AddMinutes(60),
            Verb = HttpVerb.PUT,
            // ContentType = "video/mp4"
        };
        // request.Headers["Content-Length"] = maxFileSizeBytes.ToString();
        
        string? presignedUrl = await _s3Client.GetPreSignedURLAsync(request);

        string metadataJson = JsonSerializer.Serialize(metadata);
        UploadCallbackService.MetadataByFileId[fileId] = metadataJson;
        UploadCallbackService.UriByFileId[fileId] = $"https://pub-e5bd95d5cb1c44978ee90d63f5fe8a70.r2.dev/{request.Key}";

        // await UploadRandomDataUsingPresignedUrl(presignedUrl, 1);

        return new PresignedUpload(fileId, presignedUrl);
    }

    public async Task UploadRandomDataUsingPresignedUrl(string presignedUrl, int sizeInMb)
    {
        // Generate in-memory random data
        var randomData = new byte[sizeInMb * 1024 * 1024]; // Size in bytes
        new Random().NextBytes(randomData); // Fill the array with random bytes

        // Perform the upload
        using var client = new HttpClient();
        using var memoryStream = new MemoryStream(randomData);

        var request = new HttpRequestMessage(HttpMethod.Put, presignedUrl)
        {
            Content = new StreamContent(memoryStream)
        };

        var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Content uploaded successfully.");
        }
        else
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Failed to upload content. Status Code: {response.StatusCode} {responseContent}");
        }
    }
}

public class UploadService : IUploadService
{
    private readonly IAmazonS3 _s3Client;

    public UploadService(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }

    public async Task<UploadedFile> UploadFile(string filePath, string filename, string contentType)
    {
        string fileId = Guid.NewGuid().ToString();

        var request = new GetPreSignedUrlRequest()
        {
            BucketName = "fluentuploads-test",
            Key = $"{fileId}-{filename}",
            Expires = DateTime.UtcNow.AddMinutes(60),
            Verb = HttpVerb.PUT,
            // ContentType = "video/mp4"
        };
        // request.Headers["Content-Length"] = maxFileSizeBytes.ToString();
        
        string? presignedUrl = await _s3Client.GetPreSignedURLAsync(request);

        // Upload the file to the presigned URL
        using var httpClient = new HttpClient();
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        
        var uploadRequest = new HttpRequestMessage(HttpMethod.Put, presignedUrl)
        {
            Content = new StreamContent(fileStream)
        };

        uploadRequest.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

        var response = await httpClient.SendAsync(uploadRequest);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to upload file. Status Code: {response.StatusCode}");
        }
        
        return new UploadedFile(fileId, $"https://pub-e5bd95d5cb1c44978ee90d63f5fe8a70.r2.dev/{request.Key}");
    }
}
