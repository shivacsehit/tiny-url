using System.Net.Http;
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

        public async Task<List<TinyUrlEntry>> GetPublicAsync(string search = "")
        {
            var url = string.IsNullOrEmpty(search)
                ? $"{BaseUrl}/api/public"
                : $"{BaseUrl}/api/public?search={search}";

            var result = await _http.GetFromJsonAsync<List<TinyUrlEntry>>(url);
            return result ?? new List<TinyUrlEntry>();
        }

        public async Task<string?> AddAsync(string url, bool isPrivate)
        {
            var payload = new { url, isPrivate };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _http.PostAsync($"{BaseUrl}/api/add", content);
            if (!response.IsSuccessStatusCode) return null;

            var result = await response.Content.ReadFromJsonAsync<AddResponse>();
            return result?.ShortUrl;
        }

        public async Task DeleteAsync(string code)
        {
            await _http.DeleteAsync($"{BaseUrl}/api/delete/{code}");
        }

        private class AddResponse
        {
            public string ShortUrl { get; set; } = "";
            public string Code { get; set; } = "";
        }
    }
}