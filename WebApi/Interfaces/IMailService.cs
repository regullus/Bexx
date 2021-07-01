#region using

using WebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#endregion 

namespace WebApi.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
        Task SendWelcomeEmailAsync(WelcomeRequest request);
        Task SendEsqueceuSenhaEmailAsync(EsqueceuSenhaRequest request);

    }
}
