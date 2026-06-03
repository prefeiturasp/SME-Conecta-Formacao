using FluentAssertions;
using MediatR;
using Moq;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Usuarios
{
    public class SalvarUsuarioTelefoneParcialCommandHandlerTestes
    {
        private readonly Mock<IRepositorioUsuario> repositorioUsuarioMock;
        private readonly Mock<IMediator> mediatorMock;
        private readonly SalvarUsuarioTelefoneParcialCommandHandler handler;

        public SalvarUsuarioTelefoneParcialCommandHandlerTestes()
        {
            repositorioUsuarioMock = new Mock<IRepositorioUsuario>();
            mediatorMock = new Mock<IMediator>();

            handler = new SalvarUsuarioTelefoneParcialCommandHandler(
                repositorioUsuarioMock.Object,
                mediatorMock.Object);
        }

        [Fact]
        public void Deve_lancar_excecao_quando_repositorio_for_nulo()
        {
            Action acao = () =>
                new SalvarUsuarioTelefoneParcialCommandHandler(
                    null!,
                    mediatorMock.Object);

            acao.Should()
                .Throw<ArgumentNullException>()
                .WithParameterName("repositorioUsuario");
        }

        [Fact]
        public void Deve_lancar_excecao_quando_mediator_for_nulo()
        {
            Action acao = () =>
                new SalvarUsuarioTelefoneParcialCommandHandler(
                    repositorioUsuarioMock.Object,
                    null!);

            acao.Should()
                .Throw<ArgumentNullException>()
                .WithParameterName("mediator");
        }

        [Fact]
        public async Task Deve_lancar_excecao_quando_usuario_nao_encontrado()
        {
            var command = new SalvarUsuarioTelefoneParcialCommand(
                "123456",
                "11999999999");

            repositorioUsuarioMock
                .Setup(x => x.ObterPorLogin(command.Login))
                .ReturnsAsync((Usuario)null!);

            Func<Task> acao = () =>
                handler.Handle(command, CancellationToken.None);

            await acao.Should()
                .ThrowAsync<NegocioException>()
                .WithMessage(MensagemNegocio.USUARIO_NAO_ENCONTRADO);
        }

        [Fact]
        public async Task Deve_retornar_true_quando_atualizar_usuario_com_sucesso()
        {
            var usuario = new Usuario
            {
                Id = 10,
                Login = "123456"
            };

            var command = new SalvarUsuarioTelefoneParcialCommand(
                usuario.Login,
                "(11) 99999-8888");

            repositorioUsuarioMock
                .Setup(x => x.ObterPorLogin(usuario.Login))
                .ReturnsAsync(usuario);

            repositorioUsuarioMock
                .Setup(x => x.Atualizar(It.IsAny<Usuario>()))
                .ReturnsAsync(new Usuario { Id = 10 });

            mediatorMock
                .Setup(x => x.Send(
                     It.IsAny<RemoverCacheCommand>(),
                     It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var resultado = await handler.Handle(command, CancellationToken.None);

            resultado.Should().BeTrue();

            usuario.Telefone.Should().Be("11999998888");

            mediatorMock.Verify(
                x => x.Send(
                    It.IsAny<RemoverCacheCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(2));

            repositorioUsuarioMock.Verify(
                x => x.Atualizar(It.Is<Usuario>(
                    u => u.Telefone == "11999998888")),
                Times.Once);
        }

        [Fact]
        public async Task Deve_retornar_false_quando_atualizacao_nao_persistir()
        {
            var usuario = new Usuario
            {
                Id = 10,
                Login = "123456"
            };

            var command = new SalvarUsuarioTelefoneParcialCommand(
                usuario.Login,
                "11999998888");

            repositorioUsuarioMock
                .Setup(x => x.ObterPorLogin(usuario.Login))
                .ReturnsAsync(usuario);

            repositorioUsuarioMock
                .Setup(x => x.Atualizar(It.IsAny<Usuario>()))
                .ReturnsAsync(new Usuario());

            mediatorMock
                .Setup(x => x.Send(
                     It.IsAny<RemoverCacheCommand>(),
                     It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var resultado = await handler.Handle(command, CancellationToken.None);

            resultado.Should().BeFalse();
        }
    }
}
