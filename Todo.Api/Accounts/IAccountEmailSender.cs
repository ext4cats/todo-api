namespace Todo.Api.Accounts;

public interface IAccountEmailSender
{
    Task SendVerificationEmailAsync(string recipient, string verificationLink);
    Task SendPasswordResetEmailAsync(string recipient, string resetLink);
}
