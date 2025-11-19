using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoNotificarPareceristasParaReanalise : CasoDeUsoAbstrato, ICasoDeUsoNotificarPareceristasParaReanalise
    {
        public CasoDeUsoNotificarPareceristasParaReanalise(IMediator mediator) : base(mediator)
        { }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var notificacaoPropostaPareceristasDto = param.ObterObjetoMensagem<NotificacaoPropostaPareceristasDTO>();

            if (notificacaoPropostaPareceristasDto.PropostaId == 0)
                throw new Exception(MensagemNegocio.PARAMETRO_INVALIDO);

            var proposta = await mediator.Send(new ObterPropostaPorIdQuery(notificacaoPropostaPareceristasDto.PropostaId));

            if (proposta.EhNulo())
                throw new Exception(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            if (proposta.Situacao.EstaAguardandoReanalisePeloParecerista())
                return await mediator.Send(new GerarNotificacaoReanalisePareceristaCommand(proposta, notificacaoPropostaPareceristasDto.Pareceristas));

            return false;
        }
    }
}