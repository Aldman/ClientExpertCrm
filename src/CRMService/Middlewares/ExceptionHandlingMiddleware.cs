using System.Net;
using System.Net.Mime;
using CRMService.Exceptions;

namespace CRMService.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception ex)
        {
            var (status, title) = ex switch
            {
                ArgumentException => (HttpStatusCode.BadRequest, "Invalid request"),
                ClientNotFoundException => (HttpStatusCode.NotFound, "Client not found"),
                SessionNotFoundException => (HttpStatusCode.NotFound, "Session not found"),
                InvalidDataException => (HttpStatusCode.Unauthorized, "Invalid input data"),
                _ => (HttpStatusCode.InternalServerError, "Server error")
            };
            
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = (int)status;
            
            var payload = new
            {
                status,
                title,
                details = ex.GetBaseException().Message,
                traceId = context.TraceIdentifier
            };
            
            await context.Response.WriteAsJsonAsync(payload);
        }
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}