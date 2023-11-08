using RabbitMQ.Client;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Infra.Servicos.Mensageria
{
    public interface IServicoMensageriaMetricas
    {
        Task<bool> Publicar(MetricaMensageria mensagem, string rota, string exchange, string nomeAcao, IModel canalRabbit = null);
        Task Publicado(string rota);
        Task Obtido(string rota);
        Task Concluido(string rota);
        Task Erro(string rota);
    }
}
