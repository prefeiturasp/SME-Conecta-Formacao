using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTiposAreaPromotoraQuery : IRequest<IEnumerable<AreaPromotoraTipoDTO>>
    {
    }
}
