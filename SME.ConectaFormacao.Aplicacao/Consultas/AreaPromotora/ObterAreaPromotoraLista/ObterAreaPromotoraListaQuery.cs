using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAreaPromotoraListaQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        public ObterAreaPromotoraListaQuery(long? areaPromotoraIdUsuarioLogado)
        {
            AreaPromotoraIdUsuarioLogado = areaPromotoraIdUsuarioLogado;
        }

        public long? AreaPromotoraIdUsuarioLogado { get; }
    }
}
