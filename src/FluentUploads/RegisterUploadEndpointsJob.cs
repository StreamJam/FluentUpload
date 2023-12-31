using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FluentUploads;

public class RegisterUploadEndpointsJob : BackgroundService
{
    private readonly ILogger<RegisterUploadEndpointsJob> _logger;
    private readonly EndpointDataSource _endpoints;
    private readonly IUploadCallbackService _callbackService;
    private readonly IHostApplicationLifetime _appLifetime;

    public RegisterUploadEndpointsJob(ILogger<RegisterUploadEndpointsJob> logger, EndpointDataSource endpoints, IUploadCallbackService callbackService, IHostApplicationLifetime appLifetime)
    {
        _logger = logger;
        _endpoints = endpoints;
        _callbackService = callbackService;
        _appLifetime = appLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Wait for the application to fully start
        _appLifetime.ApplicationStarted.Register(async () =>
        {
            var callbackHandlers = _endpoints.Endpoints
                .SelectMany(endpoint => endpoint.Metadata.OfType<IUploadCallbackHandler>().Select(handler => new
                {
                    CallbackId = endpoint.DisplayName ?? throw new Exception("Endpoint has no display name"),
                    Handler = handler
                }))
                .ToArray();

            await Task.WhenAll(callbackHandlers.Select(callbackHandler => _callbackService.RegisterCallback(callbackHandler.CallbackId, callbackHandler.Handler)));
            
            _logger.LogInformation("Registered {Count} upload endpoints", callbackHandlers.Length);
        });
    }
}