using MailKit.Net.Smtp;

namespace SME.ConectaFormacao.Infra.Servicos.Emails.Interfaces
{
    public interface ISmtpClientFactory
    {
        ISmtpClient Criar();
    }
}
