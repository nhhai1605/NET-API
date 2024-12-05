namespace NET_API.Entities;

public class Error : Exception
{
    public Error(int statusCode, string message)
    {
        StatusCode = statusCode;
        Message = message;
    }
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
}