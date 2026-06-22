using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioCodafSuplementar(IConectaFormacaoConexao conexao, IContextoAplicacao contexto) :
        RepositorioBaseAuditavel<CodafSuplementar>(contexto, conexao), IRepositorioCodafSuplementar
    {
    }
}