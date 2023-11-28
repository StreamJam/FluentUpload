using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FluentUploads;

public class FileUploadRouteBuilder
{
    private readonly IEndpointRouteBuilder _routeBuilder;
    private readonly string _pattern;

    public FileUploadRouteBuilder(IEndpointRouteBuilder routeBuilder, string pattern)
    {
        _routeBuilder = routeBuilder;
        _pattern = pattern;
    }

    public FileUploadRouteBuilder<TMetadata> WithFileMetadata<TMetadata>(Func<HttpContext, Task<TMetadata>> metadataFunc)
    {
        return new FileUploadRouteBuilder<TMetadata>(_routeBuilder, _pattern, metadataFunc);
    }

    public void Build()
    {
        _routeBuilder.MapPost(_pattern, async (HttpContext context) =>
        {
            
        });
    }
}

public class FileUploadRouteBuilder<TMetadata>
{
    private readonly IEndpointRouteBuilder _routeBuilder;
    private readonly string _pattern;
    private Func<HttpContext, Task<TMetadata>> _metadataFunc;
    private Func<HttpContext, TMetadata, Task>? _completionFunc;

    public FileUploadRouteBuilder(IEndpointRouteBuilder routeBuilder, string pattern, Func<HttpContext, Task<TMetadata>> metadataFunc)
    {
        _routeBuilder = routeBuilder;
        _pattern = pattern;
        _metadataFunc = metadataFunc;
    }

    public FileUploadRouteBuilder<TMetadata> OnUploadComplete(Func<HttpContext, TMetadata, Task> completionFunc)
    {
        _completionFunc = completionFunc;
        return this;
    }

    public void Build()
    {
        _routeBuilder.MapPost(_pattern, async (HttpContext context) =>
        {
            TMetadata metadata = await _metadataFunc(context);
        });
    }
}