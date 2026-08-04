using Dommel;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioLog(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : 
        RepositorioBase<Log>(contexto, conexao), 
        IRepositorioLog
    {
        public async Task<long> InserirAsync(Log log)
        {
            log.Id = (long)await conexao.Obter().InsertAsync(log);
            return log.Id;
        }
    }
}