using TinyUrl.API.Models;

namespace TinyUrl.API.Interfaces
{
    public interface IAuthService
    {
        AuthResponse? LoginWithCredentials(string username, string password);
        AuthResponse? LoginWithApiKey(string apiKey);
        string GenerateToken(string username);
    }
}