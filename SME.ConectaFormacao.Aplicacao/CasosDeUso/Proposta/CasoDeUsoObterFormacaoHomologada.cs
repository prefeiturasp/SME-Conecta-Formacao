using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterFormacaoHomologada : CasoDeUsoAbstrato, ICasoDeUsoObterFormacaoHomologada
    {
        public CasoDeUsoObterFormacaoHomologada(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<RetornoListagemDTO>> Executar()
        {
            return await mediator.Send(ObterFormacaoHomologadaQuery.Instancia);
        }
    }
}
