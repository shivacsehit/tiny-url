using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
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

        public string GetShortUrl(string code)
            => $"{_apiBaseUrl}/{code}";

        public async Task<List<TinyUrlEntry>> GetPublicAsync(string search = "")
        {
            var url = string.IsNullOrEmpty(search)
                ? $"{_apiBaseUrl}/api/urls"               
                : $"{_apiBaseUrl}/api/urls?search={search}"; 
            return await _http.GetFromJsonAsync<List<TinyUrlEntry>>(url)
                   ?? new List<TinyUrlEntry>();
        }

        public async Task<AddUrlResponse?> AddAsync(string url, bool isPrivate)
        {
            var response = await _http.PostAsJsonAsync(
                $"{_apiBaseUrl}/api/urls",   
                new { url, isPrivate });
            return await response.Content
                .ReadFromJsonAsync<AddUrlResponse>();
        }

        public async Task DeleteAsync(string code)
        {
            await _http.DeleteAsync(
                $"{_apiBaseUrl}/api/urls/{code}");  
        }

        public async Task UpdateAsync(string code, string url, bool isPrivate)
        {
            await _http.PutAsJsonAsync(
                $"{_apiBaseUrl}/api/urls/{code}",
                new { url, isPrivate });
        }
    }

    public class AddUrlResponse
    {
        public string ShortUrl { get; set; } = "";
        public string Code { get; set; } = "";
    }
}