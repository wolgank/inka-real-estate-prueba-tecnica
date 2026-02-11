
using System.Net;
using System.Text.Json;
using API.Exceptions;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IEnumerable<IExceptionHandler> _handlers;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, IEnumerable<IExceptionHandler> handlers, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _handlers = handlers;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error capturado por Middleware");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Recorremos los handlers en el orden en que fueron registrados
        var handler = _handlers.FirstOrDefault(h => h.CanHandle(exception));

        if (handler != null)
        {
            var (status, body) = handler.Handle(exception);
            context.Response.StatusCode = (int)status;
            await context.Response.WriteAsync(JsonSerializer.Serialize(body));
        }
    }
}