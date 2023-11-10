using RabbitMQ.Client;
using SME.ConectaFormacao.Infra.Servicos.Log;

namespace SME.ConectaFormacao.Infra.Servicos.Mensageria
{
    public interface IServicoMensageriaLogs
    {
        Task<bool> Publicar(LogMensagem mensagem, string rota, string exchange, string nomeAcao, IModel canalRabbit = null);
        string ObterParametrosMensagem(LogMensagem mensagemLog);
    }
}
