using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinyUrl.API.Data;
using TinyUrl.API.Helpers;

namespace TinyUrl.API.Controllers
{
    [ApiController]
    public class TinyUrlController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TinyUrlController(AppDbContext db)
        {
            _db = db;
        }

        // POST /api/add
        [HttpPost("api/add")]
        public async Task<IActionResult> Add([FromBody] TinyUrlAddDto dto)
        {
            if (string.IsNullOrEmpty(dto.Url))
                return BadRequest("URL is required");

            // Auto-add https:// if missing
            var url = dto.Url.Trim();
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                url = "https://" + url;

            string code;
            do { code = ShortCodeGenerator.Generate(); }
            while (await _db.TinyUrls.AnyAsync(x => x.ShortCode == code));

            var entry = new API.Models.TinyUrl
            {
                OriginalUrl = url,
                ShortCode = code,
                IsPrivate = dto.IsPrivate
            };

            _db.TinyUrls.Add(entry);
            await _db.SaveChangesAsync();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            return Ok(new { shortUrl = $"{baseUrl}/{code}", code });
        }

        // GET /api/public
        [HttpGet("api/public")]
        public async Task<IActionResult> GetPublic([FromQuery] string? search)
        {
            var query = _db.TinyUrls.Where(x => !x.IsPrivate);
            if (!string.IsNullOrEmpty(search))
                query = query.Where(x =>
                    x.ShortCode.Contains(search) ||
                    x.OriginalUrl.Contains(search));

            var result = await query
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(result);
        }

        // GET /{code} - redirect to original URL
        [HttpGet("{code}")]
        public async Task<IActionResult> RedirectToUrl(string code)
        {
            // Skip system routes
            if (code is "swagger" or "api" or "favicon.ico"
                or "health" or "index.html")
                return NotFound();

            var entry = await _db.TinyUrls
                .FirstOrDefaultAsync(x => x.ShortCode == code);

            if (entry is null)
                return NotFound($"Short code '{code}' not found");

            entry.Clicks++;
            await _db.SaveChangesAsync();

            return Redirect(entry.OriginalUrl);
        }

        // DELETE /api/delete/{code}
        [HttpDelete("api/delete/{code}")]
        public async Task<IActionResult> Delete(string code)
        {
            var entry = await _db.TinyUrls
                .FirstOrDefaultAsync(x => x.ShortCode == code);
            if (entry is null) return NotFound();

            _db.TinyUrls.Remove(entry);
            await _db.SaveChangesAsync();
            return Ok();
        }

        // DELETE /api/delete-all
        [HttpDelete("api/delete-all")]
        public async Task<IActionResult> DeleteAll()
        {
            _db.TinyUrls.RemoveRange(_db.TinyUrls);
            await _db.SaveChangesAsync();
            return Ok();
        }

        // PUT /api/update/{code}
        [HttpPut("api/update/{code}")]
        public async Task<IActionResult> Update(
            string code,
            [FromBody] TinyUrlAddDto dto)
        {
            var entry = await _db.TinyUrls
                .FirstOrDefaultAsync(x => x.ShortCode == code);
            if (entry is null) return NotFound();

            // Auto-add https:// if missing
            var url = dto.Url.Trim();
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                url = "https://" + url;

            entry.OriginalUrl = url;
            entry.IsPrivate = dto.IsPrivate;
            await _db.SaveChangesAsync();
            return Ok(entry);
        }
    }

    public record TinyUrlAddDto(string Url, bool IsPrivate);
}