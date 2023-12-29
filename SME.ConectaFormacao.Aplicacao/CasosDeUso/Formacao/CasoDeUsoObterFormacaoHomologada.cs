using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Formacao;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Formacao
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
