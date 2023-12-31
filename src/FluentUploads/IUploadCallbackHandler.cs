namespace FluentUploads;

public interface IUploadCallbackHandler
{
    public Task Invoke(string metadataJson);
}