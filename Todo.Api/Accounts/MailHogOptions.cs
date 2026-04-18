namespace Todo.Api.Accounts;

public class MailHogOptions
{
    public required string FromAddress { get; init; }
    public required string Host { get; init; }
    public required int Port { get; init; }
}
