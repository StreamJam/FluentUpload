namespace FluentUploads;

public class UploadContext<TMetadata> : IServiceProvider
{
    public string FileId { get; }
    public string Uri { get; } 
    public TMetadata Metadata { get; }

    private readonly IServiceProvider _serviceProvider;
    
    public UploadContext(string fileId, string uri, TMetadata metadata, IServiceProvider serviceProvider)
    {
        FileId = fileId;
        Uri = uri;
        Metadata = metadata;
        _serviceProvider = serviceProvider;
    }

    public object? GetService(Type serviceType) => _serviceProvider.GetService(serviceType);
}
