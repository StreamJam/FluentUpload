using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FluentUploads;

public static class EndpointRouteBuilderExtensions
{
    public static FileUploadRouteBuilder MapFileUpload(this IEndpointRouteBuilder endpoints, [StringSyntax("Route")] string pattern, Delegate handler)
    {
        var builder = new FileUploadRouteBuilder(endpoints, pattern);
        return builder;
    }
    
    public static RouteHandlerBuilder OnUploadComplete(this RouteHandlerBuilder builder, Func<HttpContext, Task> completionFunc)
    {
        // Store the upload completion function for later use in the pipeline
        // builder.WithMetadata(new FileUploadOptions { UploadCompletionFunc = completionFunc });
        return builder;
    }
}