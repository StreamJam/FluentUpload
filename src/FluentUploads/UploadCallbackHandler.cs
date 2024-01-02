using System.Text.Json;

namespace FluentUploads;

internal class UploadCallbackHandler<TMetadata> : IUploadCallbackHandler
{
    private readonly Func<UploadContext<TMetadata>, Task> _callback;

    public UploadCallbackHandler(Func<UploadContext<TMetadata>, Task> callback)
    {
        _callback = callback;
    }

    public async Task Invoke(string fileId, string uri, string metadataJson, IServiceProvider serviceProvider)
    {
        TMetadata? metadata = JsonSerializer.Deserialize<TMetadata>(metadataJson);
        
        if (metadata is null)
            throw new Exception("Failed to deserialize metadata.");
        
        await _callback(new UploadContext<TMetadata>(fileId, uri, metadata, serviceProvider));
    }
}
