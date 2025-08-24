using ApiService.Application;
using Microsoft.Extensions.Logging;

namespace ApiService.Infrastructure;

public class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(ILogger<EmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body)
    {
        _logger.LogInformation("Sending email to {To}. Subject: {Subject}. Body: {Body}", to, subject, body);
        return Task.CompletedTask;
    }
}

