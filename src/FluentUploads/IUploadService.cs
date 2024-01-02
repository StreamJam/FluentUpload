namespace FluentUploads;

public interface IUploadService<TMetadata>
{
    public Task<PresignedUpload> CreateUpload(TMetadata metadata, string filename, long maxFileSizeBytes = 1024 * 1024, string? callbackId = null);
}

public interface IUploadService
{
    public Task<UploadedFile> UploadFile(string filePath, string filename, string contentType);
}
