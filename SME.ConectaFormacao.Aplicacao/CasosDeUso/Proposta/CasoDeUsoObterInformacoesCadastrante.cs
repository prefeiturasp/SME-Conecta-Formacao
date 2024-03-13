using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterInformacoesCadastrante : CasoDeUsoAbstrato, ICasoDeUsoObterInformacoesCadastrante
    {
        public CasoDeUsoObterInformacoesCadastrante(IMediator mediator) : base(mediator)
        {
        }

        public async Task<PropostaInformacoesCadastranteDTO> Executar(long? propostaId)
        {
            return await mediator.Send(new ObterInformacoesCadastranteQuery(propostaId));
        }
    }
}
