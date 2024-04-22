using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class PropostasConfirmadasQueNaoEncerramAindaQuery : IRequest<IEnumerable<long>>
    {

    }
}