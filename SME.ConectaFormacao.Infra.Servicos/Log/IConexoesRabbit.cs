using RabbitMQ.Client;

namespace SME.ConectaFormacao.Infra.Servicos.Log
{
    public interface IConexoesRabbit
    {
        IModel Get();
        void Return(IModel conexao);
    }

    public interface IConexoesRabbitAcessos : IConexoesRabbit { }
    public interface IConexoesRabbitLogs : IConexoesRabbit { }
}
