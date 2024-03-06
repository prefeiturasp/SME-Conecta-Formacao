using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoGerarPropostaTurmaVaga : CasoDeUsoAbstrato, ICasoDeUsoGerarPropostaTurmaVaga
    {
        public CasoDeUsoGerarPropostaTurmaVaga(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var propostaId = long.Parse(param.Mensagem.ToString());

            var proposta = await mediator.Send(new ObterPropostaPorIdQuery(propostaId));

            if (proposta.FormacaoHomologada != FormacaoHomologada.Sim)
                return await mediator.Send(new GerarPropostaTurmaVagaCommand(propostaId, proposta.QuantidadeVagasTurma.GetValueOrDefault()));

            return false;
        }
    }
}
