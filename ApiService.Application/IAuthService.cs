using ApiService.Domain;

namespace ApiService.Application;

public interface IAuthService
{
    Task<User> RegisterAsync(string email, string password);
    Task<bool> ConfirmEmailAsync(Guid userId, string token);
}

