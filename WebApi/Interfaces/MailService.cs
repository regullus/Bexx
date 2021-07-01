#region using

using MailKit.Net.Smtp;
using MailKit.Security;
using WebApi.Model;
using Microsoft.Extensions.Options;
using MimeKit;
using System.IO;
using System.Threading.Tasks;
using Utils;
using System.Runtime.InteropServices;
using WebApi.Helper;

#endregion

namespace WebApi.Services
{
    public class MailService : IMailService
    {
        //Codigo veio de: https://codewithmukesh.com/blog/send-emails-with-aspnet-core/

        private readonly MailSettings _mailSettings;
        private const string cLocal = "MailService";

        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        public async Task SendWelcomeEmailAsync(WelcomeRequest request)
        {
            try
            {
                string FilePath = Directory.GetCurrentDirectory() + "\\Templates\\WelcomeTemplate.html";
                StreamReader str = new StreamReader(FilePath);
                string MailText = str.ReadToEnd();
                str.Close();
                MailText = MailText.Replace("[subject]", "Bem vindo!");
                MailText = MailText.Replace("[username]", request.UserName).Replace("[email]", request.ToEmail);
                MailText = MailText.Replace("[link]", request.link);
                MailText = MailText.Replace("[termos]", "https://tronar.dev/cdn/b2c/termos/tronar.pdf");
                MailText = MailText.Replace("[vejamais]", "https://tronar.com");
                MailText = Helpers.MimiHtml(MailText);

                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
                email.To.Add(MailboxAddress.Parse(request.ToEmail));
                email.Subject = $"Welcome {request.UserName}";
                var builder = new BodyBuilder();
                builder.HtmlBody = MailText;
                email.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
            }
            catch (System.Exception ex)
            {
                Geral.LogErro(cLocal, "SendWelcomeEmailAsync - " + request.UserName, ex.Message, 1);
            }
        }

        public async Task SendEsqueceuSenhaEmailAsync(EsqueceuSenhaRequest request)
        {
            try
            {
                string FilePath = "";
 
                if (Helpers.OsIsWindows())
                {
                    FilePath = Directory.GetCurrentDirectory() + @"\Templates\EsqueceuSenhaTemplate.html";
                }
                else
                {
                    FilePath = Directory.GetCurrentDirectory() + @"/Templates/EsqueceuSenhaTemplate.html";
                }
                 
                StreamReader str = new StreamReader(FilePath);
                string MailText = str.ReadToEnd();
                str.Close();
                MailText = MailText.Replace("[subject]", "Esqueceu a senha!");
                MailText = MailText.Replace("[username]", request.UserName).Replace("[email]", request.ToEmail);
                MailText = MailText.Replace("[email]", request.ToEmail);
                MailText = MailText.Replace("[link]", request.link);
                MailText = Helpers.MimiHtml(MailText);

                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
                email.To.Add(MailboxAddress.Parse(request.ToEmail));
                email.Subject = $"Esqueceu a Senha!";
                var builder = new BodyBuilder();
                builder.HtmlBody = MailText;
                email.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
            }
            catch (System.Exception ex)
            {
                Geral.LogErro(cLocal, "SendEsqueceuSenhaEmailAsync - " + request.UserName, ex.Message, 1);
            }
        }
    }
}
