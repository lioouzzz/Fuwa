using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;


public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "未處理的錯誤。Path: {Path}", httpContext.Request.Path);
        httpContext.Response.StatusCode = 500;
        await httpContext.Response.WriteAsJsonAsync(new { status = 500, message = "請求發生錯誤，請稍後再嘗試", traceId = httpContext.TraceIdentifier }, cancellationToken);
        return true;
    }
}