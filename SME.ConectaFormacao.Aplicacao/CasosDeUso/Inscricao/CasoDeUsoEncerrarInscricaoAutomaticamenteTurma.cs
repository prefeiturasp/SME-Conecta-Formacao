using MediatR;
using Newtonsoft.Json;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao;

public class CasoDeUsoEncerrarInscricaoAutomaticamenteTurma : CasoDeUsoAbstrato , ICasoDeUsoEncerrarInscricaoAutomaticamenteTurma
{
    public CasoDeUsoEncerrarInscricaoAutomaticamenteTurma(IMediator mediator) : base(mediator)
    {
    }

    public async Task<bool> Executar(MensagemRabbit param)
    {
        var propostaId = JsonConvert.DeserializeObject<long>(param.Mensagem.ToString()!);
        var turmas = await mediator.Send(new ObterPropostasTurmasPorPropostaIdQuery(propostaId));
        foreach (var turma in turmas)
        {
            await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.EncerrarInscricaoAutomaticamenteInscricoes,
                turma,Guid.NewGuid(), new Dominio.Entidades.Usuario("Sistema", "Sistema", string.Empty)));
        }
        return true;
    }
}