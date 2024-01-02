using System.Collections.Concurrent;

using Microsoft.Extensions.DependencyInjection;

namespace FluentUploads;

public class UploadCallbackService : IUploadCallbackService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ConcurrentDictionary<string, IUploadCallbackHandler> _callbacks = new();
    public static readonly ConcurrentDictionary<string, string> MetadataByFileId = new();
    public static readonly ConcurrentDictionary<string, string> UriByFileId = new();

    public UploadCallbackService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public Task RegisterCallback<TMetadata>(string callbackId, Func<UploadContext<TMetadata>, Task> callback)
    {
        var handler = new UploadCallbackHandler<TMetadata>(callback);
        return RegisterCallback(callbackId, handler);
    }

    public Task RegisterCallback(string callbackId, IUploadCallbackHandler handler)
    {
        _callbacks[callbackId] = handler;
        return Task.CompletedTask;
    }

    public async Task HandleCallback(string fileId, string uri, string callbackId, string metadataJson)
    {
        if (_callbacks.TryGetValue(callbackId, out IUploadCallbackHandler? callbackHandler))
        {
            using var scope = _scopeFactory.CreateScope();
            await callbackHandler.Invoke(fileId, uri, metadataJson, scope.ServiceProvider);
        }
        else
        {
            throw new KeyNotFoundException($"Callback '{callbackId}' not found");
        }
    }

    public async Task HandleClientUploadStatus(string fileId, bool success, string? error)
    {
        var callbackId = _callbacks.First().Key;
        var metadataJson = MetadataByFileId[fileId];
        var uri = UriByFileId[fileId];
        await HandleCallback(fileId, uri, callbackId, metadataJson);
    }
}
