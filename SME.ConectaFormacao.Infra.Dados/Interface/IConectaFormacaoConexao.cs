using System.Data;

namespace SME.ConectaFormacao.Infra.Dados;

public interface IConectaFormacaoConexao : IDisposable
{
    void Abrir();
    void Fechar();
    IDbConnection Obter();
}
