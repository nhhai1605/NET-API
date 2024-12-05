using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NET_API.Configurations;
using NET_API.Entities;

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
        return new List<Item>();
    }
}