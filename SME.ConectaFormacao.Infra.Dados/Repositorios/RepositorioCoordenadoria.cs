using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioCoordenadoria(IConectaFormacaoConexao conexao, IContextoAplicacao contexto) :
        RepositorioBaseAuditavel<Coordenadoria>(contexto, conexao), IRepositorioCoordenadoria
    {
        public async Task<Coordenadoria?> ObterComAreaPromotoraAsync(long id)
        {
            var coordenadoria = await ObterPorId(id);
            coordenadoria?.AreasPromotoras = await conexao.Obter().QueryAsync<AreaPromotora>("SELECT * FROM area_promotora WHERE coordenadoria_id = {id}", new { id });
            return coordenadoria;
        }
    }
}
