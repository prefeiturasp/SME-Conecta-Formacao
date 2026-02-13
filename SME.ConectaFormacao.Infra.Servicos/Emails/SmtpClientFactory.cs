using MailKit.Net.Smtp;
using SME.ConectaFormacao.Infra.Servicos.Emails.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Servicos.Emails
{
    [ExcludeFromCodeCoverage]
    public class SmtpClientFactory : ISmtpClientFactory
    {
        public ISmtpClient Criar() => new SmtpClient();
    }
}
