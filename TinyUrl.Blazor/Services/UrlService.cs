using System.Net.Http.Json;
using TinyUrl.Blazor.Models;

namespace TinyUrl.Blazor.Services
{
    public class UrlService
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "http://localhost:5000";

        public UrlService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<TinyUrlEntry>> GetPublicAsync(string search = "")
        {
            var url = string.IsNullOrEmpty(search)
                ? $"{BaseUrl}/api/public"
                : $"{BaseUrl}/api/public?search={search}";
            return await _http.GetFromJsonAsync<List<TinyUrlEntry>>(url)
                   ?? new List<TinyUrlEntry>();
        }

        public async Task<AddUrlResponse?> AddAsync(string url, bool isPrivate)
        {
            var response = await _http.PostAsJsonAsync(
                $"{BaseUrl}/api/add",
                new { url, isPrivate });
            return await response.Content
                .ReadFromJsonAsync<AddUrlResponse>();
        }

        public async Task DeleteAsync(string code)
        {
            await _http.DeleteAsync($"{BaseUrl}/api/delete/{code}");
        }
    }

    public class AddUrlResponse
    {
        public string ShortUrl { get; set; } = "";
        public string Code { get; set; } = "";
    }
}