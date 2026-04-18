using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Todo.Api.Accounts;

[ApiController]
[Route("[controller]")]
public class AccountController(
    UserManager<Account> userManager,
    SignInManager<Account> signInManager,
    IAccountEmailSender accountEmailSender,
    IOptions<AppOptions> appOptions)
    : ControllerBase
{
    private readonly AppOptions _appOptions = appOptions.Value;

    [HttpPost("register")]
    public async Task<Results<Ok, ValidationProblem>> Register([FromBody] RegisterRequest request)
    {
        var account = new Account { UserName = request.Email, Email = request.Email };
        var result = await userManager.CreateAsync(account, request.Password);
        if (!result.Succeeded)
            return TypedResults.ValidationProblem(result.Errors.ToDictionary(
                e => e.Code, e => new[] { e.Description }));
        var token = await userManager.GenerateEmailConfirmationTokenAsync(account);
        var verificationLink =
            $"{_appOptions.BaseUrl}${_appOptions.ConfirmEmailPath}?userId={account.Id}&token={Uri.EscapeDataString(token)}";
        await accountEmailSender.SendVerificationEmailAsync(account.Email, verificationLink);
        return TypedResults.Ok();
    }

    [HttpPost("confirm-email")]
    public async Task<Results<Ok, BadRequest>> ConfirmEmail([FromBody] ConfirmEmailRequest request)
    {
        var account = await userManager.FindByIdAsync(request.UserId);
        if (account is null) return TypedResults.BadRequest();
        var result = await userManager.ConfirmEmailAsync(account, request.Token);
        if (!result.Succeeded) return TypedResults.BadRequest();
        return TypedResults.Ok();
    }

    [HttpPost("log-in")]
    public async Task<Results<Ok, UnauthorizedHttpResult>> LogIn([FromBody] LogInRequest request)
    {
        var result = await signInManager.PasswordSignInAsync(
            request.Email, request.Password, true, true);
        if (!result.Succeeded)
            return TypedResults.Unauthorized();
        return TypedResults.Ok();
    }

    [HttpPost("forgot-password")]
    public async Task<Ok> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var account = await userManager.FindByEmailAsync(request.Email);
        if (account is null) return TypedResults.Ok();
        if (account.Email is null) throw new InvalidOperationException("Account has no email.");
        var token = await userManager.GeneratePasswordResetTokenAsync(account);
        var resetLink =
            $"{_appOptions.BaseUrl}{_appOptions.ResetPasswordPath}?userId={account.Id}&token={Uri.EscapeDataString(token)}";
        await accountEmailSender.SendPasswordResetEmailAsync(account.Email, resetLink);
        return TypedResults.Ok();
    }

    [HttpPost("reset-password")]
    public async Task<Results<Ok, ValidationProblem>> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var account = await userManager.FindByIdAsync(request.UserId);
        if (account is null)
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                { { "token", ["Invalid or expired token."] } });
        var result = await userManager.ResetPasswordAsync(account, request.Token, request.NewPassword);
        if (!result.Succeeded)
            return TypedResults.ValidationProblem(result.Errors.ToDictionary(e => e.Code,
                e => new[] { e.Description }));
        return TypedResults.Ok();
    }

    [HttpPost("resend-verification")]
    public async Task<Ok> ResendVerification([FromBody] ResendVerificationRequest request)
    {
        var account = await userManager.FindByEmailAsync(request.Email);
        if (account is null) return TypedResults.Ok();
        if (account.Email is null) throw new InvalidOperationException("Account has no email.");
        if (await userManager.IsEmailConfirmedAsync(account)) return TypedResults.Ok();
        var token = await userManager.GenerateEmailConfirmationTokenAsync(account);
        var verificationLink =
            $"{_appOptions.BaseUrl}{_appOptions.ConfirmEmailPath}?userId={account.Id}&token={Uri.EscapeDataString(token)}";
        await accountEmailSender.SendVerificationEmailAsync(account.Email, verificationLink);
        return TypedResults.Ok();
    }

    [Authorize]
    [HttpPost("log-out")]
    public async Task<Ok> LogOut()
    {
        await signInManager.SignOutAsync();
        return TypedResults.Ok();
    }

    [Authorize]
    [HttpGet("basic-info")]
    public async Task<Ok<BasicInfoResponse>> GetBasicInfo()
    {
        var account = await userManager.GetUserAsync(User)
                      ?? throw new InvalidOperationException("Logged in account not found.");
        return TypedResults.Ok(BasicInfoResponse.From(account));
    }
}

public record RegisterRequest(string Email, string Password);

public record ConfirmEmailRequest(string UserId, string Token);

public record LogInRequest(string Email, string Password);

public record ForgotPasswordRequest(string Email);

public record ResetPasswordRequest(string UserId, string Token, string NewPassword);

public record ResendVerificationRequest(string Email);

public record BasicInfoResponse(string? UserName, string? Email)
{
    public static BasicInfoResponse From(Account account)
    {
        return new BasicInfoResponse(account.UserName, account.Email);
    }
}
