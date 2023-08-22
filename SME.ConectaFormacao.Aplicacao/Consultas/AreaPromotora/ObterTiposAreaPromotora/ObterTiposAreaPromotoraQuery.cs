using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTiposAreaPromotoraQuery : IRequest<IEnumerable<AreaPromotoraTipoDTO>>
    {
    }
}
