using System.Net;
using System.Text.Json;

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
        catch (Exception ex)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            var errorMessage = JsonSerializer.Serialize(new { Message = ex.Message });
            await httpContext.Response.WriteAsync(errorMessage);
        }
    }

}