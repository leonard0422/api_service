using ApiService.Domain;

namespace ApiService.Application;

public interface IUserRepository
{
    Task<User> CreateAsync(User user);
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid id);
    Task UpdateEmailVerificationTokenAsync(Guid userId, string token);
    Task UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiry);
    Task SetEmailVerifiedAsync(Guid userId);
}
