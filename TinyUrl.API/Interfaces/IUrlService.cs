using TinyUrl.API.Models;

namespace TinyUrl.API.Interfaces
{
    // D: Abstraction for business logic
    public interface IUrlService
    {
        Task<List<TinyUrl.API.Models.TinyUrl>> GetPublicUrlsAsync(string? search);
        Task<TinyUrl.API.Models.TinyUrl> CreateShortUrlAsync(string url, bool isPrivate, string baseUrl);
        Task<TinyUrl.API.Models.TinyUrl?> GetByCodeAsync(string code);
        Task<string?> GetOriginalUrlAsync(string code);
        Task<TinyUrl.API.Models.TinyUrl?> UpdateUrlAsync(string code, string url, bool isPrivate);
        Task<bool> DeleteUrlAsync(string code);
        Task DeleteAllUrlsAsync();
    }
}