using TinyUrl.API.Interfaces;
using TinyUrl.API.Models;

namespace TinyUrl.API.Services
{
    public class UrlService : IUrlService
    {
        private readonly IUrlRepository _repo;
        private readonly IShortCodeGenerator _codeGenerator;

        public UrlService(
            IUrlRepository repo,
            IShortCodeGenerator codeGenerator)
        {
            _repo = repo;
            _codeGenerator = codeGenerator;
        }

        public async Task<List<TinyUrl.API.Models.TinyUrl>> GetPublicUrlsAsync(string? search) =>
            await _repo.GetPublicAsync(search);

        public async Task<TinyUrl.API.Models.TinyUrl> CreateShortUrlAsync(
            string url, bool isPrivate, string baseUrl)
        {
            // Auto-add https:// if missing
            url = url.Trim();
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                url = "https://" + url;

            // Generate unique code
            string code;
            do { code = _codeGenerator.Generate(); }
            while (await _repo.CodeExistsAsync(code));

            var entry = new TinyUrl.API.Models.TinyUrl
            {
                OriginalUrl = url,
                ShortCode = code,
                ShortUrl = $"{baseUrl}/{code}",
                IsPrivate = isPrivate
            };

            return await _repo.AddAsync(entry);
        }

        public async Task<TinyUrl.API.Models.TinyUrl?> GetByCodeAsync(string code) =>
            await _repo.GetByCodeAsync(code);

        public async Task<string?> GetOriginalUrlAsync(string code)
        {
            var entry = await _repo.GetByCodeAsync(code);
            if (entry is null) return null;

            await _repo.IncrementClicksAsync(code);
            return entry.OriginalUrl;
        }

        public async Task<TinyUrl.API.Models.TinyUrl?> UpdateUrlAsync(
            string code, string url, bool isPrivate)
        {
            // Auto-add https:// if missing
            url = url.Trim();
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                url = "https://" + url;

            return await _repo.UpdateAsync(code, url, isPrivate);
        }

        public async Task<bool> DeleteUrlAsync(string code) =>
            await _repo.DeleteAsync(code);

        public async Task DeleteAllUrlsAsync() =>
            await _repo.DeleteAllAsync();
    }
}