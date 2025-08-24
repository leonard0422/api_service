using ApiService.Application;
using ApiService.Domain;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Infrastructure;

public class UserRepository : IUserRepository
{
    private readonly ApiDbContext _context;

    public UserRepository(ApiDbContext context)
    {
        _context = context;
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public Task<User?> GetByIdAsync(Guid id)
    {
        return _context.Users.FindAsync(id).AsTask();
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        return _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is not null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateEmailVerificationTokenAsync(Guid userId, string token)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is not null)
        {
            user.EmailVerificationToken = token;
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiry)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is not null)
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = expiry;
            await _context.SaveChangesAsync();
        }
    }

    public async Task SetEmailVerifiedAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is not null)
        {
            user.IsEmailVerified = true;
            user.EmailVerificationToken = null;
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdatePasswordResetTokenAsync(Guid userId, string token, DateTime expiry)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is not null)
        {
            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiry = expiry;
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdatePasswordAsync(Guid userId, string passwordHash)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is not null)
        {
            user.PasswordHash = passwordHash;
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;
            await _context.SaveChangesAsync();
        }
    }
}
