using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterCriterioValidacaoInscricao : CasoDeUsoAbstrato, ICasoDeUsoObterCriterioValidacaoInscricao
    {
        public CasoDeUsoObterCriterioValidacaoInscricao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<CriterioValidacaoInscricaoDTO>> Executar()
        {
            return await mediator.Send(ObterCriterioValidacaoInscricaoQuery.Instancia);
        }
    }
}
