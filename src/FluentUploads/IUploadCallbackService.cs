namespace FluentUploads;

public interface IUploadCallbackService
{
    public Task RegisterCallback<TMetadata>(string callbackId, Func<TMetadata, Task> callback);
    public Task RegisterCallback(string callbackId, IUploadCallbackHandler handler);
    public Task HandleCallback(string callbackId, string metadataJson);
}