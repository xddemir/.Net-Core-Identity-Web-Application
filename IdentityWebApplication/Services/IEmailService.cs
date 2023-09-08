namespace IdentityWebApplication.Services;

public interface IEmailService
{
    Task SendResetPasswordEmailLink(string resetEmailLink, string toEmail);

}