using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoNotificarPareceristasSobreAtribuicaoPelaDF : CasoDeUsoAbstrato, ICasoDeUsoNotificarPareceristasSobreAtribuicaoPelaDF
    {
        public CasoDeUsoNotificarPareceristasSobreAtribuicaoPelaDF(IMediator mediator) : base(mediator)
        {}

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var notificacaoPropostaPareceristasDto = param.ObterObjetoMensagem<NotificacaoPropostaPareceristasDTO>();
            
            if (notificacaoPropostaPareceristasDto.PropostaId == 0)
                throw new Exception(MensagemNegocio.PARAMETRO_INVALIDO);
            
            var proposta = await mediator.Send(new ObterPropostaPorIdQuery(notificacaoPropostaPareceristasDto.PropostaId));

            if (proposta.EhNulo())
                throw new Exception(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);
            
            if (proposta.Situacao.EstaAguardandoAnalisePeloParecerista())
                return await mediator.Send(new GerarNotificacaoPareceristaCommand(proposta, notificacaoPropostaPareceristasDto.Pareceristas));

            return false;
        }
    }
}