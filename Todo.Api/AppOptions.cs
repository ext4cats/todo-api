namespace Todo.Api;

public class AppOptions
{
    public required string BaseUrl { get; init; }
    public required string ConfirmEmailPath { get; init; }
    public required string ResetPasswordPath { get; init; }
}
