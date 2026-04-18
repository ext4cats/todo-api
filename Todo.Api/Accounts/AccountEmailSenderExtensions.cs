namespace Todo.Api.Accounts;

public static class AccountEmailSenderExtensions
{
    public static IServiceCollection AddAccountEmailSender(this IServiceCollection services,
        IConfiguration configuration)
    {
        var transport = configuration["Email:Transport"];
        switch (transport)
        {
            case "MailHog":
                services.AddOptions<MailHogOptions>()
                    .BindConfiguration("Email:MailHog")
                    .ValidateOnStart();
                services.AddTransient<IAccountEmailSender, MailHogAccountEmailSender>();
                break;
            default:
                throw new InvalidOperationException($"Unknown email transport: {transport}");
        }

        return services;
    }
}
