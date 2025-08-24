using ApiService.Application;
using ApiService.Domain;

namespace ApiService.Infrastructure;

public class UserRepository : IUserRepository
{
    private readonly Dictionary<Guid, User> _users = new();

    public Task<User> CreateAsync(User user)
    {
        _users[user.Id] = user;
        return Task.FromResult(user);
    }

    public Task<User?> GetByIdAsync(Guid id)
    {
        _users.TryGetValue(id, out var user);
        return Task.FromResult(user);
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        var user = _users.Values.FirstOrDefault(u => u.Email == email);
        return Task.FromResult(user);
    }

    public Task<IEnumerable<User>> GetAllAsync()
    {
        IEnumerable<User> users = _users.Values;
        return Task.FromResult(users);
    }

    public Task UpdateAsync(User user)
    {
        if (_users.ContainsKey(user.Id))
        {
            _users[user.Id] = user;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        _users.Remove(id);
        return Task.CompletedTask;
    }

    public Task UpdateEmailVerificationTokenAsync(Guid userId, string token)
    {
        if (_users.TryGetValue(userId, out var user))
        {
            user.EmailVerificationToken = token;
        }
        return Task.CompletedTask;
    }

    public Task UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiry)
    {
        if (_users.TryGetValue(userId, out var user))
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = expiry;
        }
        return Task.CompletedTask;
    }

    public Task SetEmailVerifiedAsync(Guid userId)
    {
        if (_users.TryGetValue(userId, out var user))
        {
            user.IsEmailVerified = true;
            user.EmailVerificationToken = null;
        }
        return Task.CompletedTask;
    }

    public Task UpdatePasswordResetTokenAsync(Guid userId, string token, DateTime expiry)
    {
        if (_users.TryGetValue(userId, out var user))
        {
            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiry = expiry;
        }
        return Task.CompletedTask;
    }

    public Task UpdatePasswordAsync(Guid userId, string passwordHash)
    {
        if (_users.TryGetValue(userId, out var user))
        {
            user.PasswordHash = passwordHash;
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;
        }
        return Task.CompletedTask;
    }
}
