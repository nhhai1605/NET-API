using System.Net;
using System.Text.Json;
using NET_API.Entities;

namespace NET_API.Middlewares;

public class HandleExceptionMiddleware
{
    private readonly RequestDelegate _next;
    public HandleExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Error err)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = err.StatusCode;
            var errorMessage = JsonSerializer.Serialize(new { Message = err.Message });
            await httpContext.Response.WriteAsync(errorMessage);
        }
    }

}