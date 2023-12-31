using System.Text;
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

    public async Task<string> CreateUpload(TMetadata metadata, string filename, long maxFileSizeBytes = 1024 * 1024, string? callbackId = null)
    {
        string? displayName = _httpContextAccessor?.HttpContext?.GetEndpoint()?.DisplayName;

        var request = new GetPreSignedUrlRequest()
        {
            BucketName = "fluentuploads-test",
            Key = $"{Guid.NewGuid().ToString()}-{filename}",
            Expires = DateTime.UtcNow.AddMinutes(60),
            Verb = HttpVerb.PUT,
            ContentType = "image/jpeg"
        };
        request.Headers["Content-Length"] = maxFileSizeBytes.ToString();
        
        string? presignedUrl = await _s3Client.GetPreSignedURLAsync(request);

        await UploadRandomDataUsingPresignedUrl(presignedUrl, 1);

        return presignedUrl;
    }
    
    public async Task UploadRandomDataUsingPresignedUrl(string presignedUrl, int sizeInMB)
    {
        // Generate in-memory random data
        var randomData = new byte[sizeInMB * 1024 * 1024]; // Size in bytes
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