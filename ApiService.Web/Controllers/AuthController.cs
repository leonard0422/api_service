using ApiService.Application;
using Microsoft.AspNetCore.Mvc;

namespace ApiService.Web.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        await _authService.RegisterAsync(request.Email, request.Password);
        return Ok(new { message = "Registration successful. Check your email to confirm." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var tokens = await _authService.LoginAsync(request.Email, request.Password);
        if (tokens == null)
        {
            return Unauthorized();
        }
        return Ok(tokens);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var tokens = await _authService.RefreshTokenAsync(request.RefreshToken);
        if (tokens == null)
        {
            return Unauthorized();
        }
        return Ok(tokens);
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery] string token)
    {
        var success = await _authService.ConfirmEmailAsync(userId, token);
        if (!success)
        {
            return BadRequest("Invalid token.");
        }

        return Ok("Email confirmed.");
    }
}

public record RegisterRequest(string Email, string Password);
public record LoginRequest(string Email, string Password);
public record RefreshTokenRequest(string RefreshToken);

