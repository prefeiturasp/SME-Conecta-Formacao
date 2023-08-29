using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterTipoInscricao : CasoDeUsoAbstrato, ICasoDeUsoObterTipoInscricao
    {
        public CasoDeUsoObterTipoInscricao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<RetornoListagemDTO>> Executar()
        {
            return await mediator.Send(ObterTipoFormacaoQuery.Instancia);
        }
    }
}
