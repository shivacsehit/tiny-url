using TinyUrl.API.Models;

namespace TinyUrl.API.Interfaces
{
    // I: Read-only operations separated
    public interface IReadUrlRepository
    {
        Task<List<TinyUrl.API.Models.TinyUrl>> GetPublicAsync(string? search);
        Task<TinyUrl.API.Models.TinyUrl?> GetByCodeAsync(string code);
        Task<bool> CodeExistsAsync(string code);
    }
}