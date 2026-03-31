using TinyUrl.API.Models;

namespace TinyUrl.API.Interfaces
{
    // I: Write operations separated
    public interface IWriteUrlRepository
    {
        Task<TinyUrl.API.Models.TinyUrl> AddAsync(TinyUrl.API.Models.TinyUrl entry);
        Task<TinyUrl.API.Models.TinyUrl?> UpdateAsync(string code, string url, bool isPrivate);
        Task<bool> DeleteAsync(string code);
        Task DeleteAllAsync();
        Task IncrementClicksAsync(string code);
    }
}