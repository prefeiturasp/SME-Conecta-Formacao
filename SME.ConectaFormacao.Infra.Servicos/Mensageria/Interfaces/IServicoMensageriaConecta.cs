using RabbitMQ.Client;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Infra.Servicos.Mensageria
{
    public interface IServicoMensageriaConecta
    {
        Task<bool> Publicar(MensagemRabbit mensagem, string rota, string exchange, string nomeAcao, IModel canalRabbit = null);
    }
}
