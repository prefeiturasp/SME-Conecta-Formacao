using MediatR;
using Newtonsoft.Json;
using SME.ConectaFormacao.Aplicacao.CasosDeUso;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao
{
    public class CasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes : CasoDeUsoAbstrato, ICasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes
    {
        public CasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var turmaId = JsonConvert.DeserializeObject<long>(param.Mensagem.ToString()!);
            var inscricoes = await mediator.Send(new ObtertInscricoesPorPropostaTurmaQuery(turmaId));
            foreach (var inscricao in inscricoes)
            {
                await mediator.Send(
                    new PublicarNaFilaRabbitCommand(RotasRabbit.EncerrarInscricaoAutomaticamenteUsuarios, inscricao));
            }
            return true;
        }
    }
}