using MediatR;
using Newtonsoft.Json;
using SME.ConectaFormacao.Aplicacao.CasosDeUso;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao
{
    public class CasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes : CasoDeUsoAbstrato,
        ICasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes
    {
        public CasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var turmaIds = param.ObterObjetoMensagem<TurmasIdParaCancelamentoDTO>();
            var inscricoes = await mediator.Send(new ObtertInscricoesPorPropostaTurmaQuery(turmaIds.TurmasIds));
            if (inscricoes.Any())
            {
                await mediator.Send(
                    new PublicarNaFilaRabbitCommand(RotasRabbit.EncerrarInscricaoAutomaticamenteUsuarios, inscricoes,
                        Guid.NewGuid(), new Dominio.Entidades.Usuario("Sistema", "Sistema", string.Empty)));
            }

            return true;
        }
    }
}