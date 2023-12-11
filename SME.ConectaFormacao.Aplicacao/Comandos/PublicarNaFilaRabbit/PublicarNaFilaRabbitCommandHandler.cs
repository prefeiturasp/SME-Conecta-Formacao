using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Log;
using SME.ConectaFormacao.Infra.Servicos.Mensageria;

namespace SME.ConectaFormacao.Aplicacao
{
    public class PublicarNaFilaRabbitCommandHandler : IRequestHandler<PublicarNaFilaRabbitCommand, bool>
    {
        private readonly IMediator mediator;
        private readonly IServicoMensageriaConecta servicoMensageria;
        private readonly IServicoMensageriaMetricas servicoMensageriaMetricas;

        public PublicarNaFilaRabbitCommandHandler(IMediator mediator, IServicoMensageriaConecta servicoMensageria, IServicoMensageriaMetricas servicoMensageriaMetricas)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.servicoMensageria = servicoMensageria ?? throw new ArgumentNullException(nameof(servicoMensageria));
            this.servicoMensageriaMetricas = servicoMensageriaMetricas ?? throw new ArgumentNullException(nameof(servicoMensageriaMetricas));
        }
        public PublicarNaFilaRabbitCommandHandler(IMediator mediator, IServicoMensageriaConecta servicoMensageria)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.servicoMensageria = servicoMensageria ?? throw new ArgumentNullException(nameof(servicoMensageria));
        }
        public async Task<bool> Handle(PublicarNaFilaRabbitCommand command, CancellationToken cancellationToken)
        {
            var usuario = command.Usuario ?? await ObterUsuario();
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
        private async Task<Usuario> ObterUsuario()
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
}