using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoNotificarAreaPromotoraSobreValidacaoFinalPelaDF : CasoDeUsoAbstrato, ICasoDeUsoNotificarAreaPromotoraSobreValidacaoFinalPelaDF
    {
        public CasoDeUsoNotificarAreaPromotoraSobreValidacaoFinalPelaDF(IMediator mediator) : base(mediator)
        {}

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var propostaId = long.Parse(param.Mensagem.ToString());
            
            if (propostaId == 0)
                throw new Exception(MensagemNegocio.PARAMETRO_INVALIDO);
            
            var proposta = await mediator.Send(new ObterPropostaPorIdQuery(propostaId));

            if (proposta.EhNulo())
                throw new Exception(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);
            
            if (proposta.Situacao.EstaAprovadaOuRecusada())
                return await mediator.Send(new GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommand(proposta));

            return false;
        }
    }
}