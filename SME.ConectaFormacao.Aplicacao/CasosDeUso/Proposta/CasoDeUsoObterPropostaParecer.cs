using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterPropostaParecer : CasoDeUsoAbstrato, ICasoDeUsoObterPropostaParecer
    {
        public CasoDeUsoObterPropostaParecer(IMediator mediator) : base(mediator)
        {}
        
        public async Task<PropostaPareceristaConsideracaoCompletoDTO> Executar(PropostaParecerFiltroDTO propostaParecerFiltroDto)
        {
            return await mediator.Send(new ObterPropostaParecerPorPropostaIdECampoQuery(propostaParecerFiltroDto.PropostaId, propostaParecerFiltroDto.Campo));
        }
    }
}