using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoRecusarPropostaParecerista : CasoDeUsoAbstrato, ICasoDeUsoRecusarPropostaParecerista
    {
        public CasoDeUsoRecusarPropostaParecerista(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(long propostaId, PropostaJustificativaDTO propostaJustificativaDTO)
        {
            if (propostaJustificativaDTO.Justificativa.NaoEstaPreenchido())
                throw new NegocioException(MensagemNegocio.JUSTIFICATIVA_NAO_INFORMADA);

            var usuarioLogado = await mediator.Send(ObterUsuarioLogadoQuery.Instancia());
            return await mediator.Send(new EnviarParecerPareceristaCommand(propostaId, usuarioLogado.Login, SituacaoParecerista.Recusada, propostaJustificativaDTO.Justificativa));
        }
    }
}
