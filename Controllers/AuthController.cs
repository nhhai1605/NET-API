using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NET_API.Configurations;
using NET_API.Entities;
using Npgsql;

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
        try
        {
            using (NpgsqlDataSource datasource = NpgsqlDataSource.Create(_appConfig.PostgresConnection))
            {
                using var connection = datasource.OpenConnection();
                using (var command = new NpgsqlCommand("SELECT * FROM auth.login(@username, @password)", connection))
                {
                    command.Parameters.AddWithValue("@username", loginRequest.Username);
                    command.Parameters.AddWithValue("@password", loginRequest.Password);
                    var reader = command.ExecuteReader();
                    List<User> users = new List<User>();
                    while (reader.Read())
                    {
                        users.Add(Utils.Utils.Map<User>(reader));
                    }
                    if(users.Count == 0)
                    {
                        throw new Error((int)HttpStatusCode.Unauthorized, "Unauthorized");
                    }
                    if (users.Count > 1)
                    {
                        throw new Error((int)HttpStatusCode.InternalServerError, "Internal Server Error");
                    }
                    User user = users.First();
                    string token = Utils.Utils.EncryptAES($"{user.Id}~{user.Username}~{user.Password}~{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}", _appConfig.SecretKey);
                    return token;
                }
            }
        }
        catch (Exception e)
        {
            throw new Error((int)HttpStatusCode.InternalServerError, e.Message);
        }
    }
}