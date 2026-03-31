using System.Net.Http.Headers;
using System.Net.Http.Json;
using TinyUrl.Blazor.Models;

namespace TinyUrl.Blazor.Services
{
    public class UrlService
    {
        private readonly HttpClient _http;
        private readonly string _apiBaseUrl;

        public UrlService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _apiBaseUrl = config["ApiSettings:BaseUrl"]
                ?? "http://localhost:5000";
        }

        public void SetToken(string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public string GetShortUrl(string code)
            => $"{_apiBaseUrl}/{code}";

        // ✅ Auto login
        public async Task<AuthResponse?> AutoLoginAsync()
        {
            try
            {
                var response = await _http.PostAsJsonAsync(
                    $"{_apiBaseUrl}/api/auth/login",
                    new { username = "admin", password = "Admin@123" });

                if (!response.IsSuccessStatusCode) return null;

                var result = await response.Content
                    .ReadFromJsonAsync<AuthResponse>();

                if (result != null)
                    SetToken(result.Token);

                return result;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<TinyUrlEntry>> GetPublicAsync(
            string search = "")
        {
            var url = string.IsNullOrEmpty(search)
                ? $"{_apiBaseUrl}/api/urls"
                : $"{_apiBaseUrl}/api/urls?search={search}";
            return await _http.GetFromJsonAsync<List<TinyUrlEntry>>(url)
                   ?? new List<TinyUrlEntry>();
        }

        public async Task<AddUrlResponse?> AddAsync(
            string url, bool isPrivate)
        {
            var response = await _http.PostAsJsonAsync(
                $"{_apiBaseUrl}/api/urls",
                new { url, isPrivate });
            return await response.Content
                .ReadFromJsonAsync<AddUrlResponse>();
        }

        public async Task DeleteAsync(string code)
        {
            await _http.DeleteAsync($"{_apiBaseUrl}/api/urls/{code}");
        }
    }

    public class AuthResponse
    {
        public string Token { get; set; } = "";
        public string Username { get; set; } = "";
        public int ExpiresIn { get; set; }
    }

    public class AddUrlResponse
    {
        public string ShortUrl { get; set; } = "";
        public string Code { get; set; } = "";
    }
}