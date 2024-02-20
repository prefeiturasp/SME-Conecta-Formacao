using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoCancelarInscricao : CasoDeUsoAbstrato, ICasoDeUsoCancelarInscricao
    {
        public CasoDeUsoCancelarInscricao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(long id)
        {
            return await mediator.Send(new CancelarInscricaoCommand(id));
        }
    }
}
