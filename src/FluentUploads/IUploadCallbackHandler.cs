namespace FluentUploads;

public interface IUploadCallbackHandler
{
    public Task Invoke(string fileId, string uri, string metadataJson, IServiceProvider serviceProvider);
}
