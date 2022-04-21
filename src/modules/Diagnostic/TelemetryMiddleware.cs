using Microsoft.AspNetCore.Http;

namespace Ws.Core.Extensions.Diagnostic;

public class TelemetryMiddleware
{
    private readonly RequestDelegate _next;

    public TelemetryMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // trace

        await _next(context);
    }
}

