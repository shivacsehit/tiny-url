namespace TinyUrl.API.Models
{
    public record LoginRequest(string Username, string Password);
    public record ApiKeyRequest(string ApiKey);

    public record AuthResponse(
        string Token,
        string TokenType,
        int ExpiresIn,
        string Username
    );
}