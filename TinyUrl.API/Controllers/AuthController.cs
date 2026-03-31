using Microsoft.AspNetCore.Mvc;
using TinyUrl.API.Interfaces;
using TinyUrl.API.Models;

namespace TinyUrl.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST /api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var response = _authService.LoginWithCredentials(
                request.Username, request.Password);

            if (response is null)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(response);
        }

        // POST /api/auth/apikey
        [HttpPost("apikey")]
        public IActionResult LoginWithApiKey([FromBody] ApiKeyRequest request)
        {
            var response = _authService.LoginWithApiKey(request.ApiKey);

            if (response is null)
                return Unauthorized(new { message = "Invalid API key" });

            return Ok(response);
        }

        // GET /api/auth/validate
        [HttpGet("validate")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult Validate()
        {
            return Ok(new
            {
                valid = true,
                username = User.Identity?.Name
            });
        }
    }
}