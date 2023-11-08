using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.SalvarLogViaRabbit;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Log;
using SME.ConectaFormacao.Infra.Servicos.Mensageria;

namespace SME.ConectaFormacao.Aplicacao;

public class SalvarLogViaRabbitCommandHandler : IRequestHandler<SalvarLogViaRabbitCommand, bool>
{
    private readonly IServicoMensageriaLogs servicoMensageria;

    public SalvarLogViaRabbitCommandHandler(IServicoMensageriaLogs servicoMensageria)
    {
        this.servicoMensageria = servicoMensageria ?? throw new ArgumentNullException(nameof(servicoMensageria));
    }

    public async Task<bool> Handle(SalvarLogViaRabbitCommand request, CancellationToken cancellationToken)
    {
        var mensagem = new LogMensagem(
            request.Mensagem,
            request.Contexto.ToString(),
            request.Nivel.ToString(),
            request.Observacao,
            request.Rastreamento,
            request.Projeto,
            request.ExcecaoInterna);
        return await servicoMensageria.Publicar(mensagem, RotasRabbitLogs.RotaLogs, ExchangeRabbit.Logs, "PublicarFilaLog");
    }
}