using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NET_API.Configurations;
using NET_API.Entities;
using Npgsql;

namespace NET_API.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemController : ControllerBase
{
    private readonly AppConfig _appConfig;
    public ItemController(IOptions<AppConfig> appConfig)
    {
        _appConfig = appConfig.Value;
    }
    
    //example of a get request
    [HttpGet]
    [Route(nameof(List))]
    public List<Item> List()
    {
        try
        {
            List<Item> items = new List<Item>();
            using (NpgsqlDataSource datasource = NpgsqlDataSource.Create(_appConfig.PostgresConnection))
            {
                using var connection = datasource.OpenConnection();
                using (var command = new NpgsqlCommand("SELECT * FROM item.item", connection))
                {
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        items.Add(Utils.Utils.Map<Item>(reader));
                    }
                }
            }
            return items;
        }
        catch (Exception e)
        {
            throw new Error((int)HttpStatusCode.InternalServerError, e.Message);
        }
    }
}