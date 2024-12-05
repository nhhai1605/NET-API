using System.Net;
using Microsoft.Extensions.Options;
using NET_API.Configurations;

namespace NET_API.Middlewares;

public class AuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AppConfig _appConfig;
    public AuthorizationMiddleware(RequestDelegate next, IOptions<AppConfig> appConfig)
    {
        _next = next;
        _appConfig = appConfig.Value;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        string? token = httpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(token) || !IsTokenValid(token))
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await httpContext.Response.WriteAsync("Unauthorized");
            return;
        }
        await _next(httpContext);
    }
    
    public bool IsTokenValid(string token)
    {
        try
        {
            string decryptedString = Utils.Utils.DecryptAES(token, _appConfig.SecretKey);
            string[] split = decryptedString.Split("~");
            if (split.Length != 4)
            {
                return false;
            }
            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - Convert.ToInt64(split[3]) > _appConfig.TokenExpirySeconds)
            {
                return false;
            }
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
