using AhorrahogarApi.Constants;
using AhorrahogarApi.Models;
using Microsoft.AspNetCore.Diagnostics;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context, Exception ex, CancellationToken ct)
    {
        _logger.LogError(ex, "Error no controlado");

        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(
            new Response<object>(false, HttpMessages.InternalServerError), ct);

        return true;
    }
}