using System.Text.Json;

namespace FluentUploads;

internal class UploadCallbackHandler<TMetadata> : IUploadCallbackHandler
{
    private readonly Func<TMetadata, Task> _callback;

    public UploadCallbackHandler(Func<TMetadata, Task> callback)
    {
        _callback = callback;
    }

    public async Task Invoke(string metadataJson)
    {
        TMetadata? metadata = JsonSerializer.Deserialize<TMetadata>(metadataJson);
        
        if (metadata is null)
            throw new Exception("Failed to deserialize metadata.");
        
        await _callback(metadata);
    }
}