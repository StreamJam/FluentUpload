using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace FluentUploads;

public class UploadEndpointMetadata<TMetadata> : IUploadCallbackHandler
{
    public Func<UploadContext<TMetadata>, Task> CompletionFunc { get; }

    public UploadEndpointMetadata(Func<UploadContext<TMetadata>, Task> completionFunc)
    {
        CompletionFunc = completionFunc;
    }
    
    public async Task Invoke(string fileId, string uri, string metadataJson, IServiceProvider serviceProvider)
    {
        if (CompletionFunc is null)
            throw new Exception("No completion function was registered.");
        
        TMetadata? metadata = JsonSerializer.Deserialize<TMetadata>(metadataJson, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        
        if (metadata is null)
            throw new Exception("Failed to deserialize metadata.");
        
        await CompletionFunc(new UploadContext<TMetadata>(fileId, uri, metadata, serviceProvider));
    }
}
