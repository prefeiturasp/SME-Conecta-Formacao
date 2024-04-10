using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoDevolverProposta : CasoDeUsoAbstrato, ICasoDeUsoDevolverProposta
    {
        public CasoDeUsoDevolverProposta(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(long propostaId, string justificativa)
        {
            var proposta = await mediator.Send(new ObterPropostaPorIdQuery(propostaId));
            if (proposta.EhNulo() || proposta.Excluido)
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            if (string.IsNullOrEmpty(justificativa))
                throw new NegocioException(MensagemNegocio.JUSTIFICATIVA_NAO_INFORMADA);

            await mediator.Send(new AlterarSituacaoDaPropostaCommand(propostaId, SituacaoProposta.Devolvida));

            return await mediator.Send(new SalvarPropostaMovimentacaoCommand(propostaId, SituacaoProposta.Devolvida, justificativa));
        }
    }
}