using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Log;
using SME.ConectaFormacao.Infra.Servicos.Mensageria;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit
{
    public class PublicarNaFilaRabbitCommandHandler(IMediator mediator, IServicoMensageriaConecta servicoMensageria, IServicoMensageriaMetricas servicoMensageriaMetricas) : IRequestHandler<PublicarNaFilaRabbitCommand, bool>
    {
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
                return await mediator.Send(ObterUsuarioLogadoQuery.Instancia());
            }
            catch
            {
                return new Usuario();
            }
        }
    }
}