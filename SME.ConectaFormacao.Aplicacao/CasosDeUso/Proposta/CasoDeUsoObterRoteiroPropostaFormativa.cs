using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterRoteiroPropostaFormativa : CasoDeUsoAbstrato, ICasoDeUsoObterRoteiroPropostaFormativa
    {
        public CasoDeUsoObterRoteiroPropostaFormativa(IMediator mediator) : base(mediator)
        {
        }

        public async Task<RoteiroPropostaFormativaDTO> Executar()
        {
            return await mediator.Send(ObterRoteiroPropostaFormativaQuery.Instancia);
        }
    }
}
