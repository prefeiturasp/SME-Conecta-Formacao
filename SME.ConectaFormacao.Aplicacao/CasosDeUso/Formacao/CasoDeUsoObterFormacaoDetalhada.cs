using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Formacao;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Formacao
{
    public class CasoDeUsoObterFormacaoDetalhada : CasoDeUsoAbstrato, ICasoDeUsoObterFormacaoDetalhada
    {
        public CasoDeUsoObterFormacaoDetalhada(IMediator mediator) : base(mediator)
        { }

        public async Task<RetornoFormacaoDetalhadaDTO> Executar(long propostaId)
        {
            return await mediator.Send(new ObterFormacaoDetalhadaPorIdQuery(propostaId));
        }
    }
}
