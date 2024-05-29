using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoPodeRealizarSorteioPorId : CasoDeUsoAbstrato, ICasoDeUsoPodeRealizarSorteioPorId
    {
        public CasoDeUsoPodeRealizarSorteioPorId(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(long id)
        {
            return await mediator.Send(new PodeRealizarSorteioInscricoesPorIdQuery(id));
        }
    }
}