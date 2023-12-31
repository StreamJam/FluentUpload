namespace FluentUploads;

public interface IUploadService<TMetadata>
{
    public Task<string> CreateUpload(TMetadata metadata, string filename, long maxFileSizeBytes = 1024 * 1024, string? callbackId = null);
}