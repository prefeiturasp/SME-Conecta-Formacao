using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoNotificarResponsavelDFSobreReanaliseDoParecerista : CasoDeUsoAbstrato, ICasoDeUsoNotificarResponsavelDFSobreReanaliseDoParecerista
    {
        public CasoDeUsoNotificarResponsavelDFSobreReanaliseDoParecerista(IMediator mediator) : base(mediator)
        { }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var notificacaoPropostaPareceristasDto = param.ObterObjetoMensagem<NotificacaoPropostaPareceristaDTO>();

            if (notificacaoPropostaPareceristasDto.PropostaId == 0)
                throw new Exception(MensagemNegocio.PARAMETRO_INVALIDO);

            var proposta = await mediator.Send(new ObterPropostaPorIdQuery(notificacaoPropostaPareceristasDto.PropostaId));

            if (proposta.EhNulo())
                throw new Exception(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            if (proposta.RfResponsavelDf.NaoEstaPreenchido())
                throw new Exception(MensagemNegocio.RESPONSAVEL_DF_NAO_ENCONTRADO);

            if (proposta.Situacao.EstaAguardandoReanalisePeloParecerista() || proposta.Situacao.EstaAguardandoAnaliseParecerFinalPelaDF())
                return await mediator.Send(new GerarNotificacaoResponsavelDFCommand(proposta, notificacaoPropostaPareceristasDto.Parecerista));

            return false;
        }
    }
}