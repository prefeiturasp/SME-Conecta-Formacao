using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioAreaPromotora : RepositorioBaseAuditavel<AreaPromotora>, IRepositorioAreaPromotora
    {
        public RepositorioAreaPromotora(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }
    }
}
