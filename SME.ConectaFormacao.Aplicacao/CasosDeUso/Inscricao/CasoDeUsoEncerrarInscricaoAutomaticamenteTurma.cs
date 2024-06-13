using MediatR;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao;

public class CasoDeUsoEncerrarInscricaoAutomaticamenteTurma : CasoDeUsoAbstrato, ICasoDeUsoEncerrarInscricaoAutomaticamenteTurma
{
    public CasoDeUsoEncerrarInscricaoAutomaticamenteTurma(IMediator mediator) : base(mediator)
    {
    }

    public async Task<bool> Executar(MensagemRabbit param)
    {
        var mensagem = param.Mensagem.ToString();
        if (mensagem == null)
            return false;        
        
        var propostaId = mensagem.JsonParaObjeto<long>();
        var turmas = await mediator.Send(new ObterPropostasTurmasPorPropostaIdQuery(propostaId));
        foreach (var turma in turmas)
        {
            await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.EncerrarInscricaoAutomaticamenteInscricoes,
                turma, Guid.NewGuid(), new Dominio.Entidades.Usuario("Sistema", "Sistema", string.Empty)));
        }
        return true;
    }
}