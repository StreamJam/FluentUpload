using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OneOf;

namespace FluentUploads;

public static class EndpointRouteBuilderExtensions
{
    public delegate Task<IValueHttpResult<TResult>> CustomHandlerDelegate<TResult>();

    public delegate Task<GenericHack<TResult, IResult>> CustomHandlerDelegate<TResult, T1>(T1 p1);
    
    public static FileUploadRouteBuilder<TMetadata> MapFileUpload<TMetadata>(this IEndpointRouteBuilder endpoints, 
        [StringSyntax("Route")] string pattern, 
        CustomHandlerDelegate<TMetadata> handler)
    {
        var builder = new FileUploadRouteBuilder<TMetadata>(endpoints, pattern, handler);
        return builder;
    }
    
    public static FileUploadRouteBuilder<TMetadata> MapFileUpload<TMetadata, T1>(this IEndpointRouteBuilder endpoints, 
        [StringSyntax("Route")] string pattern, 
        CustomHandlerDelegate<TMetadata, T1> handler)
    {
        var builder = new FileUploadRouteBuilder<TMetadata>(endpoints, pattern, handler);
        return builder;
    }
}