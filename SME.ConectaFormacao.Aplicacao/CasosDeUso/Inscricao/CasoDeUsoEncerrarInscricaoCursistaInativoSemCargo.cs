using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoEncerrarInscricaoCursistaInativoSemCargo : CasoDeUsoAbstrato, ICasoDeUsoEncerrarInscricaoCursistaInativoSemCargo
    {
        public CasoDeUsoEncerrarInscricaoCursistaInativoSemCargo(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var propostasIds = await mediator.Send(new PropostasConfirmadasQueNaoEncerramAindaQuery());

            foreach (var id in propostasIds)
            {
                await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.EncerrarInscricaoAutomaticamenteTurma, id, Guid.NewGuid(), new Dominio.Entidades.Usuario("Sistema", "Sistema", string.Empty)));
            }

            return true;
        }
    }
}