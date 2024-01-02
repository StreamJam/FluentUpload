namespace FluentUploads;

public interface IUploadCallbackService
{
    public Task RegisterCallback<TMetadata>(string callbackId, Func<UploadContext<TMetadata>, Task> callback);
    public Task RegisterCallback(string callbackId, IUploadCallbackHandler handler);
    public Task HandleCallback(string fileId, string uri, string callbackId, string metadataJson);
    public Task HandleClientUploadStatus(string fileId, bool success, string? error);
}
