namespace NET_API.Configurations;

public class AppConfig
{
    public string PostgresConnection { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public int TokenExpirySeconds  { get; set; }
}