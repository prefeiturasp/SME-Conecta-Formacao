using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoNotificarProposta : CasoDeUsoAbstrato, ICasoDeUsoNotificarProposta
    {
        public CasoDeUsoNotificarProposta(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var propostaId = long.Parse(param.Mensagem.ToString());
            
            if (propostaId == 0)
                throw new Exception(MensagemNegocio.PARAMETRO_INVALIDO);
            
            var proposta = await mediator.Send(new ObterPropostaPorIdQuery(propostaId));

            if (propostaId.EhNulo())
                throw new Exception(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);
            
            if (proposta.Situacao.EstaAguardandoAnaliseDfOuPareceristaOuParecerPelaDFOuAreaPromotoraOuAnaliseFinalPelaDF())
                return await mediator.Send(new GerarNotificacaoCommand(proposta));

            return false;
        }
    }
}