using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OneOf;

namespace FluentUploads;

public static class EndpointRouteBuilderExtensions
{
    public static RouteHandlerBuilder HandleFileUpload<TMetadata>(this RouteHandlerBuilder builder, 
        Func<UploadContext<TMetadata>, Task> uploadCompleteHandler)
    {
        builder.WithMetadata(new UploadEndpointMetadata<TMetadata>(uploadCompleteHandler));
        return builder;
    }
}
