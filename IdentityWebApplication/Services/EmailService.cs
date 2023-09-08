using System.Net;
using System.Net.Mail;
using IdentityWebApplication.OptionsModel;
using Microsoft.Extensions.Options;

namespace IdentityWebApplication.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> options)
    {
        _emailSettings = options.Value;
    }

    public async Task SendResetPasswordEmailLink(string resetEmailLink, string toEmail)
    {
        var smtpClient = new SmtpClient();
        smtpClient.Host = _emailSettings.Host;
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Port = 587;
        smtpClient.Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password);
        smtpClient.EnableSsl = true;
        
        var mailMessage = new MailMessage();
        mailMessage.From = new MailAddress(_emailSettings.Email);
        mailMessage.To.Add(toEmail);
        mailMessage.Subject = "Local Host Reset Password";
        mailMessage.Body = @$"<h4>To reset password, click the link below </h4>
            <p>
               <a href='{resetEmailLink}'> 
                    Reset Password Link
               </a> 
        </p>";

        mailMessage.IsBodyHtml = true;

        await smtpClient.SendMailAsync(mailMessage);
    }
}