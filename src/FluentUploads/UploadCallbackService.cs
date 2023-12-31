using System.Collections.Concurrent;

namespace FluentUploads;

public class UploadCallbackService : IUploadCallbackService
{
    private readonly ConcurrentDictionary<string, IUploadCallbackHandler> _callbacks = new();

    public Task RegisterCallback<TMetadata>(string callbackId, Func<TMetadata, Task> callback)
    {
        var handler = new UploadCallbackHandler<TMetadata>(callback);
        return RegisterCallback(callbackId, handler);
    }

    public Task RegisterCallback(string callbackId, IUploadCallbackHandler handler)
    {
        _callbacks[callbackId] = handler;
        return Task.CompletedTask;
    }

    public async Task HandleCallback(string callbackId, string metadataJson)
    {
        if (_callbacks.TryGetValue(callbackId, out IUploadCallbackHandler? callbackHandler))
        {
            await callbackHandler.Invoke(metadataJson);
        }
        else
        {
            throw new KeyNotFoundException($"Callback '{callbackId}' not found");
        }
    }
}