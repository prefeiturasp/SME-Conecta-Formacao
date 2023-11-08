using MediatR;
using SME.ConectaFormacao.Dominio;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Log;

namespace SME.ConectaFormacao.Aplicacao;

public class PublicarNaFilaRabbitCommandHandler : IRequestHandler<PublicarNaFilaRabbitCommand, bool>
{
    private readonly IMediator mediator;
    private readonly IServicoMensageriaConecta servicoMensageria;
    private readonly IServicoMensageriaMetricas servicoMensageriaMetricas;

    public PublicarNaFilaRabbitCommandHandler(IMediator mediator, IServicoMensageriaConecta servicoMensageria, IServicoMensageriaMetricas servicoMensageriaMetricas)
    {
        this.mediator = mediator ?? throw new ArgumentException(nameof(mediator));
        this.servicoMensageria = servicoMensageria ?? throw new ArgumentException(nameof(servicoMensageria));
        ;
        this.servicoMensageriaMetricas = servicoMensageriaMetricas ?? throw new ArgumentException(nameof(servicoMensageriaMetricas));
    }

    public async Task<bool> Handle(PublicarNaFilaRabbitCommand command, CancellationToken cancellationToken)
    {
        var usuario = command.Usuario ?? await ObtenhaUsuario();
        var mensagem = new MensagemRabbit(command.Filtros,
                                            command.CodigoCorrelacao,
                                            usuario?.Nome,
                                            usuario?.Login,
                                            null,
                                            command.NotificarErroUsuario);
        await servicoMensageria.Publicar(mensagem, command.Rota, command.Exchange ?? ExchangeRabbit.Conecta, "PublicarFilaConecta");
        await servicoMensageriaMetricas.Publicado(command.Rota);
        return true;
    }
    private async Task<Usuario> ObtenhaUsuario()
    {
        try
        {
            return await mediator.Send(ObterUsuarioLogadoQuery.Instancia);
        } 
        catch
        {
            return new Usuario();
        }
    }
}