using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoAprovarPropostaParecerista : CasoDeUsoAbstrato, ICasoDeUsoAprovarPropostaParecerista
    {
        public CasoDeUsoAprovarPropostaParecerista(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(long propostaId, PropostaJustificativaDTO propostaJustificativaDTO)
        { 
            var usuarioLogado = await mediator.Send(ObterUsuarioLogadoQuery.Instancia());
            return await mediator.Send(new EnviarParecerPareceristaCommand(propostaId, usuarioLogado.Login, SituacaoParecerista.Aprovada, propostaJustificativaDTO.Justificativa));
        }
    }
}
