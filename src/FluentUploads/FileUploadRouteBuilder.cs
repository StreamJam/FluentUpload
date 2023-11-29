using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FluentUploads;


public class FileUploadRouteBuilder<TMetadata>
{
    private readonly IEndpointRouteBuilder _routeBuilder;
    private readonly string _pattern;
    private Func<HttpContext, TMetadata, Task>? _completionFunc;
    private readonly Delegate _handler;

    public FileUploadRouteBuilder(IEndpointRouteBuilder routeBuilder, string pattern, Delegate handler)
    {
        _routeBuilder = routeBuilder;
        _pattern = pattern;
        _handler = handler;
    }

    public FileUploadRouteBuilder<TMetadata> OnUploadComplete(Func<HttpContext, TMetadata, Task> completionFunc)
    {
        _completionFunc = completionFunc;
        return this;
    }

    public void Build()
    {
        _routeBuilder.MapPost(_pattern, _handler);
    }
}