using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace FluentUploads;

public class FileUploadRouteBuilder<TMetadata> : IEndpointConventionBuilder
{
    private readonly IEndpointRouteBuilder _routeBuilder;
    private readonly IEndpointConventionBuilder _conventionBuilder;
    private Func<TMetadata, IServiceProvider, Task>? _completionFunc;

    public FileUploadRouteBuilder(IEndpointRouteBuilder routeBuilder, string pattern, Delegate handler)
    {
        _routeBuilder = routeBuilder;
        _conventionBuilder = routeBuilder.MapPost(pattern, handler);;
    }

    public FileUploadRouteBuilder<TMetadata> WithCompletionHandler(Func<TMetadata, IServiceProvider, Task> completionFunc)
    {
        _completionFunc = completionFunc;
        return this;
    }

    public void Add(Action<EndpointBuilder> convention) => _conventionBuilder.Add(convention);
}
