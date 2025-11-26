using Microsoft.AspNetCore.Diagnostics;
using FlightInformationApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FlightInformationApi.Infrastructure;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception has occurred.");

        var response = new ApiSingleResponse<object>(null, false, exception.Message);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
