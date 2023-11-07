using RabbitMQ.Client;
using SME.ConectaFormacao.Infra.Servicos.Log;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Infra
{
    public interface IServicoMensageria<T>
    where T : class
    {
        Task<bool> Publicar(T mensagem, string rota, string exchange, string nomeAcao, IModel canalRabbit = null);
    }
    public interface IServicoMensageriaConecta : IServicoMensageria<MensagemRabbit> { }
    public interface IServicoMensageriaLogs : IServicoMensageria<LogMensagem> { }
    public interface IServicoMensageriaMetricas : IServicoMensageria<MetricaMensageria>
    {
        Task Publicado(string rota);
        Task Obtido(string rota);
        Task Concluido(string rota);
        Task Erro(string rota);
    }
}
