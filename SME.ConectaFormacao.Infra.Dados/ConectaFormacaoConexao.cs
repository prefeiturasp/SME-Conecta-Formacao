using Dommel;
using Npgsql;
using System.Data;
using System.Diagnostics;

namespace SME.ConectaFormacao.Infra.Dados;

public class ConectaFormacaoConexao : IConectaFormacaoConexao
{
    private readonly IDbConnection _conexao;

    public ConectaFormacaoConexao(string stringConexao)
    {
        _conexao = new NpgsqlConnection(stringConexao);

        RegistrarLogDommel();

        Abrir();
    }

    private static void RegistrarLogDommel()
    {
#if DEBUG

        DommelMapper.LogReceived = delegate (string s)
        {
            Debug.WriteLine(s);
        };
#endif
    }

    public ConectaFormacaoConexao(IDbConnection conexao)
    {
        _conexao = conexao;
    }

    public void Dispose()
    {
        if (_conexao.State == ConnectionState.Open)
            _conexao.Close();

        GC.SuppressFinalize(this);
    }  

    public void Abrir()
    {
        if (_conexao.State != ConnectionState.Open)
            _conexao.Open();
    }

    public void Fechar()
    {
        if (_conexao.State != ConnectionState.Closed)
        {
            _conexao.Close();
        }
    }

    public IDbConnection Obter()
    {
        return _conexao;
    }
}
