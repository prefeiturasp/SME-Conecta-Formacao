using System.Data;

namespace SME.ConectaFormacao.Infra.Dados;

public class Transacao : ITransacao
{
    private readonly IConectaFormacaoConexao _conexao;

    public Transacao(IConectaFormacaoConexao conexao)
    {
        _conexao = conexao;
    }

    public IDbTransaction Iniciar()
    {
        return _conexao.Obter().BeginTransaction();
    }

    public IDbTransaction Iniciar(IsolationLevel il)
    {
        return _conexao.Obter().BeginTransaction(il);
    }
}
