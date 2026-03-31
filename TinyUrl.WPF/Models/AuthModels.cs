namespace TinyUrl.WPF.Models
{
    public class AuthResponse
    {
        public string Token { get; set; } = "";
        public string Username { get; set; } = "";
        public int ExpiresIn { get; set; }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class ApiKeyRequest
    {
        public string ApiKey { get; set; } = "";
    }
}