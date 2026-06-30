using Dommel;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioLog : RepositorioBase<Log>, IRepositorioLog
    {
        public RepositorioLog(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }
        public async Task<long> Inserir(IDbTransaction transacao, Log log)
        {
            log.Id = (long)await conexao.Obter().InsertAsync(log, transacao);
            return log.Id;
        }
    }
}