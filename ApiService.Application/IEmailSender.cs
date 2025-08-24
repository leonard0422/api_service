namespace ApiService.Application;

public interface IEmailSender
{
    Task SendEmailAsync(string to, string subject, string body);
}

