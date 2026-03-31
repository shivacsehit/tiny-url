using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TinyUrl.WPF.Models;

namespace TinyUrl.WPF.Services
{
    public class UrlService
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "http://localhost:5000";

        public UrlService()
        {
            _http = new HttpClient();
        }

        public void SetToken(string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public void ClearToken()
        {
            _http.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<AuthResponse?> LoginAsync(
            string username, string password)
        {
            var response = await _http.PostAsJsonAsync(
                $"{BaseUrl}/api/auth/login",
                new { username, password });

            if (!response.IsSuccessStatusCode) return null;
            return await response.Content
                .ReadFromJsonAsync<AuthResponse>();
        }

        public async Task<AuthResponse?> LoginWithApiKeyAsync(string apiKey)
        {
            var response = await _http.PostAsJsonAsync(
                $"{BaseUrl}/api/auth/apikey",
                new { apiKey });

            if (!response.IsSuccessStatusCode) return null;
            return await response.Content
                .ReadFromJsonAsync<AuthResponse>();
        }

        public async Task<List<TinyUrlEntry>> GetPublicAsync(string search = "")
        {
            var url = string.IsNullOrEmpty(search)
                ? $"{BaseUrl}/api/urls"
                : $"{BaseUrl}/api/urls?search={search}";
            var result = await _http.GetFromJsonAsync<List<TinyUrlEntry>>(url);
            return result ?? new List<TinyUrlEntry>();
        }

        public async Task<string?> AddAsync(string url, bool isPrivate)
        {
            var json = JsonSerializer.Serialize(new { url, isPrivate });
            var content = new StringContent(
                json, Encoding.UTF8, "application/json");
            var response = await _http.PostAsync(
                $"{BaseUrl}/api/urls", content);
            if (!response.IsSuccessStatusCode) return null;
            var result = await response.Content
                .ReadFromJsonAsync<AddResponse>();
            return result?.ShortUrl;
        }

        public async Task DeleteAsync(string code)
        {
            await _http.DeleteAsync($"{BaseUrl}/api/urls/{code}");
        }

        private class AddResponse
        {
            public string ShortUrl { get; set; } = "";
            public string Code { get; set; } = "";
        }
    }
}