using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Todo.Api.Accounts;

[ApiController]
[Route("[controller]")]
public class AccountController(UserManager<Account> userManager, SignInManager<Account> signInManager)
    : ControllerBase
{
    [HttpPost("register")]
    public async Task<Results<Ok, ValidationProblem>> Register([FromBody] RegisterRequest request)
    {
        var account = new Account { UserName = request.Email, Email = request.Email };
        var result = await userManager.CreateAsync(account, request.Password);
        if (!result.Succeeded)
            return TypedResults.ValidationProblem(result.Errors.ToDictionary(
                e => e.Code, e => new[] { e.Description }));
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
                      ?? throw new InvalidOperationException("Logged in user not found.");
        return TypedResults.Ok(BasicInfoResponse.From(account));
    }
}

public record RegisterRequest(string Email, string Password);

public record LogInRequest(string Email, string Password);

public record BasicInfoResponse(string? UserName, string? Email)
{
    public static BasicInfoResponse From(Account account)
    {
        return new BasicInfoResponse(account.UserName, account.Email);
    }
}
