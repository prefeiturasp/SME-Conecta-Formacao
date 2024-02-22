using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterInscricaoTipoListaQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
    
    }
}