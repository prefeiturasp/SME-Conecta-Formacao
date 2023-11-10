using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoEnviarPropostaParaDf: CasoDeUsoAbstrato, ICasoDeUsoEnviarPropostaParaDf
    {
        public CasoDeUsoEnviarPropostaParaDf(IMediator mediator) : base(mediator)
        {
        }

        public async Task Executar(long propostaId)
        {
            var proposta = await mediator.Send(new ObterPropostaPorIdQuery(propostaId));
            if (proposta.Situacao !=  SituacaoProposta.Cadastrada)
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ESTAR_COMO_CADASTRADA);

            await mediator.Send(new EnviarPropostaParaDfCommand(propostaId));
        }
    }
}