using ApiService.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;

namespace ApiService.Application;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IEmailSender emailSender, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _emailSender = emailSender;
        _configuration = configuration;
    }

    public async Task<User> RegisterAsync(string email, string password)
    {
        var existing = await _userRepository.GetByEmailAsync(email);
        if (existing != null)
        {
            throw new InvalidOperationException("User already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password)),
            IsEmailVerified = false,
            EmailVerificationToken = Guid.NewGuid().ToString()
        };

        await _userRepository.CreateAsync(user);

        var body = $"Use the token to verify your email: {user.EmailVerificationToken}";
        await _emailSender.SendEmailAsync(user.Email, "Verify your email", body);

        return user;
    }

    public async Task<bool> ConfirmEmailAsync(Guid userId, string token)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || user.EmailVerificationToken != token)
        {
            return false;
        }

        await _userRepository.SetEmailVerifiedAsync(userId);
        return true;
    }

    public async Task<AuthTokens?> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            return null;
        }

        var hash = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
        if (user.PasswordHash != hash || !user.IsEmailVerified)
        {
            return null;
        }

        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            }),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);

        var refreshToken = Guid.NewGuid().ToString();
        var expiry = DateTime.UtcNow.AddDays(7);
        await _userRepository.UpdateRefreshTokenAsync(user.Id, refreshToken, expiry);

        return new AuthTokens(accessToken, refreshToken);
    }

    public async Task<AuthTokens?> RefreshTokenAsync(string refreshToken)
    {
        var users = await _userRepository.GetAllAsync();
        var user = users.FirstOrDefault(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiry > DateTime.UtcNow);
        if (user == null)
        {
            return null;
        }

        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            }),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);

        var newRefreshToken = Guid.NewGuid().ToString();
        var expiry = DateTime.UtcNow.AddDays(7);
        await _userRepository.UpdateRefreshTokenAsync(user.Id, newRefreshToken, expiry);

        return new AuthTokens(accessToken, newRefreshToken);
    }
}

