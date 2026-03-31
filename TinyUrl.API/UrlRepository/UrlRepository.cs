using Microsoft.EntityFrameworkCore;
using TinyUrl.API.Data;
using TinyUrl.API.Interfaces;
using TinyUrl.API.Models;

namespace TinyUrl.API.Repositories
{
    public class UrlRepository : IUrlRepository
    {
        private readonly AppDbContext _db;

        public UrlRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<TinyUrl.API.Models.TinyUrl>> GetPublicAsync(string? search)
        {
            var query = _db.TinyUrls.Where(x => !x.IsPrivate);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(x =>
                    x.ShortCode.Contains(search) ||
                    x.OriginalUrl.Contains(search));

            return await query
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<TinyUrl.API.Models.TinyUrl?> GetByCodeAsync(string code) =>
            await _db.TinyUrls
                .FirstOrDefaultAsync(x => x.ShortCode == code);

        public async Task<TinyUrl.API.Models.TinyUrl> AddAsync(TinyUrl.API.Models.TinyUrl entry)
        {
            _db.TinyUrls.Add(entry);
            await _db.SaveChangesAsync();
            return entry;
        }

        public async Task<TinyUrl.API.Models.TinyUrl?> UpdateAsync(
            string code, string url, bool isPrivate)
        {
            var entry = await GetByCodeAsync(code);
            if (entry is null) return null;

            entry.OriginalUrl = url;
            entry.IsPrivate = isPrivate;
            await _db.SaveChangesAsync();
            return entry;
        }

        public async Task<bool> DeleteAsync(string code)
        {
            var entry = await GetByCodeAsync(code);
            if (entry is null) return false;

            _db.TinyUrls.Remove(entry);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task DeleteAllAsync()
        {
            _db.TinyUrls.RemoveRange(_db.TinyUrls);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> CodeExistsAsync(string code) =>
            await _db.TinyUrls.AnyAsync(x => x.ShortCode == code);

        public async Task IncrementClicksAsync(string code)
        {
            var entry = await GetByCodeAsync(code);
            if (entry is null) return;

            entry.Clicks++;
            await _db.SaveChangesAsync();
        }
    }
}