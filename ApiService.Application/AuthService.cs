using ApiService.Domain;

namespace ApiService.Application;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailSender _emailSender;

    public AuthService(IUserRepository userRepository, IEmailSender emailSender)
    {
        _userRepository = userRepository;
        _emailSender = emailSender;
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
}

