using ApiService.Domain;

namespace ApiService.Application;

public interface IAuthService
{
    Task<User> RegisterAsync(string email, string password);
    Task<bool> ConfirmEmailAsync(Guid userId, string token);
    Task<AuthTokens?> LoginAsync(string email, string password);
    Task<AuthTokens?> RefreshTokenAsync(string refreshToken);
}

public record AuthTokens(string AccessToken, string RefreshToken);

