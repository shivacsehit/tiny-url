using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TinyUrl.API.Interfaces;
using TinyUrl.API.Models;

namespace TinyUrl.API.Controllers
{
    [ApiController]
    public class TinyUrlController : ControllerBase
    {
        private readonly IUrlService _svc;

        public TinyUrlController(IUrlService svc)
        {
            _svc = svc;
        }

        private string GetBaseUrl() =>
            $"{Request.Scheme}://{Request.Host}";

        // Public — no auth needed
        [HttpGet("api/urls")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublic([FromQuery] string? search)
        {
            var result = await _svc.GetPublicUrlsAsync(search);
            return Ok(result);
        }

        // Public — redirect no auth needed
        [HttpGet("{code}")]
        [AllowAnonymous]
        public async Task<IActionResult> RedirectToUrl(string code)
        {
            if (code is "swagger" or "api" or "favicon.ico"
                or "health" or "index.html")
                return NotFound();

            var originalUrl = await _svc.GetOriginalUrlAsync(code);
            if (originalUrl is null)
                return NotFound($"Short code '{code}' not found");

            return Redirect(originalUrl);
        }

        // 🔒 Protected — auth required
        [HttpPost("api/urls")]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] TinyUrlAddDto dto)
        {
            if (string.IsNullOrEmpty(dto.Url))
                return BadRequest("URL is required");

            var entry = await _svc.CreateShortUrlAsync(
                dto.Url, dto.IsPrivate, GetBaseUrl());

            return CreatedAtAction(nameof(GetByCode),
                new { code = entry.ShortCode },
                new { entry.ShortUrl, entry.ShortCode, entry.Id });
        }

        // 🔒 Protected
        [HttpGet("api/urls/{code}")]
        [Authorize]
        public async Task<IActionResult> GetByCode(string code)
        {
            var entry = await _svc.GetByCodeAsync(code);
            if (entry is null) return NotFound();
            return Ok(entry);
        }

        // 🔒 Protected
        [HttpDelete("api/urls/{code}")]
        [Authorize]
        public async Task<IActionResult> Delete(string code)
        {
            var deleted = await _svc.DeleteUrlAsync(code);
            if (!deleted) return NotFound();
            return NoContent();
        }

        // 🔒 Protected
        [HttpDelete("api/urls")]
        [Authorize]
        public async Task<IActionResult> DeleteAll()
        {
            await _svc.DeleteAllUrlsAsync();
            return NoContent();
        }

        // 🔒 Protected
        [HttpPut("api/urls/{code}")]
        [Authorize]
        public async Task<IActionResult> Update(
            string code,
            [FromBody] TinyUrlAddDto dto)
        {
            var entry = await _svc.UpdateUrlAsync(
                code, dto.Url, dto.IsPrivate);
            if (entry is null) return NotFound();
            return Ok(entry);
        }
    }

    public record TinyUrlAddDto(string Url, bool IsPrivate);
}