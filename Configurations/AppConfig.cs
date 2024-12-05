namespace NET_API.Configurations;

public class AppConfig
{
    public AppConfig()
    {

    }
    public string SecretKey { get; set; } 
    public int TokenExpirySeconds  { get; set; }
}