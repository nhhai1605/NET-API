using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NET_API.Configurations;
using NET_API.Entities;

namespace NET_API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppConfig _appConfig;
    public AuthController(IOptions<AppConfig> appConfig)
    {
        _appConfig = appConfig.Value;
    }


    [HttpPost]
    [Route(nameof(Login))]
    public string Login([FromBody] LoginRequest loginRequest)
    {
        if (string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
        {
            throw new Exception("Invalid username or password");
        }
        return "token";
    }
}