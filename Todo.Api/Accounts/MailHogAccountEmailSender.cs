using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Todo.Api.Accounts;

public class MailHogAccountEmailSender(IOptions<MailHogOptions> options) : IAccountEmailSender
{
    private readonly MailHogOptions _options = options.Value;

    public async Task SendVerificationEmailAsync(string recipient, string verificationLink)
    {
        await SendAsync(recipient, "New account confirmation",
            $"<a href='{verificationLink}'>Click here to confirm your account.</a>");
    }

    public async Task SendPasswordResetEmailAsync(string recipient, string resetLink)
    {
        await SendAsync(recipient, "Password reset confirmation",
            $"<a href='{resetLink}'>Click here to reset your password.</a>");
    }

    private async Task SendAsync(string recipient, string subject, string html)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(_options.FromAddress));
        message.To.Add(MailboxAddress.Parse(recipient));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = html };

        using var client = new SmtpClient();
        await client.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.None);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
