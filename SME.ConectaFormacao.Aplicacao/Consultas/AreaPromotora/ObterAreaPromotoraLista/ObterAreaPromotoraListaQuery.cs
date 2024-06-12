using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAreaPromotoraListaQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        public ObterAreaPromotoraListaQuery(long? areaPromotoraIdUsuarioLogado, AreaPromotoraTipo? tipo = null)
        {
            AreaPromotoraIdUsuarioLogado = areaPromotoraIdUsuarioLogado;
            Tipo = tipo;
        }

        public long? AreaPromotoraIdUsuarioLogado { get; }
        public AreaPromotoraTipo? Tipo { get; }
    }
}
