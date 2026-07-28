using MediatR;
using Moq;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Usuarios
{
    public class SalvarUsuarioParcialCommandHandlerTestes
    {
        private const string Login = "52998224725";

        private readonly Mock<IRepositorioUsuario> repositorioUsuarioMock;
        private readonly Mock<IMediator> mediatorMock;
        private readonly SalvarUsuarioParcialCommandHandler sut;

        public SalvarUsuarioParcialCommandHandlerTestes()
        {
            repositorioUsuarioMock = new Mock<IRepositorioUsuario>();
            mediatorMock = new Mock<IMediator>();
            sut = new SalvarUsuarioParcialCommandHandler(
                repositorioUsuarioMock.Object,
                mediatorMock.Object);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Handle_Quando_nome_nao_for_preenchido_Deve_lancar_negocio_exception(
            string? nome)
        {
            var request = new SalvarUsuarioParcialCommand(Login, nome!);

            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => sut.Handle(request, CancellationToken.None));

            Assert.Equal(MensagemNegocio.NOME_USUARIO_NAO_PREENCHIDO, excecao.Message);
            repositorioUsuarioMock.Verify(
                r => r.ObterPorLogin(It.IsAny<string>()),
                Times.Never);
            repositorioUsuarioMock.Verify(
                r => r.Atualizar(It.IsAny<Usuario>()),
                Times.Never);
            mediatorMock.Verify(
                m => m.Send(It.IsAny<RemoverCacheCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_Quando_usuario_nao_for_encontrado_Deve_lancar_negocio_exception()
        {
            var request = new SalvarUsuarioParcialCommand(Login, "Maria da Silva");

            repositorioUsuarioMock
                .Setup(r => r.ObterPorLogin(Login))
                .ReturnsAsync((Usuario)null!);

            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => sut.Handle(request, CancellationToken.None));

            Assert.Equal(MensagemNegocio.USUARIO_NAO_ENCONTRADO, excecao.Message);
            repositorioUsuarioMock.Verify(r => r.ObterPorLogin(Login), Times.Once);
            repositorioUsuarioMock.Verify(
                r => r.Atualizar(It.IsAny<Usuario>()),
                Times.Never);
            mediatorMock.Verify(
                m => m.Send(It.IsAny<RemoverCacheCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_Deve_aplicar_trim_no_nome_e_nome_social_e_remover_os_dois_caches()
        {
            var usuario = CriarUsuario();
            var request = new SalvarUsuarioParcialCommand(Login, "  Maria da Silva  ")
            {
                NomeSocial = "  Maria Souza  "
            };
            var cancellationToken = new CancellationTokenSource().Token;

            repositorioUsuarioMock
                .Setup(r => r.ObterPorLogin(Login))
                .ReturnsAsync(usuario);
            repositorioUsuarioMock
                .Setup(r => r.Atualizar(usuario))
                .ReturnsAsync(usuario);

            var resultado = await sut.Handle(request, cancellationToken);

            Assert.True(resultado);
            Assert.Equal("Maria da Silva", usuario.Nome);
            Assert.Equal("Maria Souza", usuario.NomeSocial);

            mediatorMock.Verify(
                m => m.Send(
                    It.Is<RemoverCacheCommand>(c =>
                        c.Chave == CacheDistribuidoNomes.Usuario.Parametros(Login)),
                    cancellationToken),
                Times.Once);
            mediatorMock.Verify(
                m => m.Send(
                    It.Is<RemoverCacheCommand>(c =>
                        c.Chave == CacheDistribuidoNomes.UsuarioLogado.Parametros(Login)),
                    cancellationToken),
                Times.Once);
            repositorioUsuarioMock.Verify(r => r.Atualizar(usuario), Times.Once);
        }

        [Fact]
        public async Task Handle_Quando_nome_social_for_nulo_Deve_mante_lo_nulo()
        {
            var usuario = CriarUsuario();
            var request = new SalvarUsuarioParcialCommand(Login, "Maria da Silva")
            {
                NomeSocial = null
            };

            repositorioUsuarioMock
                .Setup(r => r.ObterPorLogin(Login))
                .ReturnsAsync(usuario);
            repositorioUsuarioMock
                .Setup(r => r.Atualizar(usuario))
                .ReturnsAsync(usuario);

            var resultado = await sut.Handle(request, CancellationToken.None);

            Assert.True(resultado);
            Assert.Null(usuario.NomeSocial);
        }

        [Fact]
        public async Task Handle_Quando_repositorio_retornar_id_zero_Deve_retornar_false()
        {
            var usuario = CriarUsuario();
            var usuarioAtualizado = CriarUsuario(id: 0);
            var request = new SalvarUsuarioParcialCommand(Login, "Maria da Silva");

            repositorioUsuarioMock
                .Setup(r => r.ObterPorLogin(Login))
                .ReturnsAsync(usuario);
            repositorioUsuarioMock
                .Setup(r => r.Atualizar(usuario))
                .ReturnsAsync(usuarioAtualizado);

            var resultado = await sut.Handle(request, CancellationToken.None);

            Assert.False(resultado);
        }

        [Fact]
        public async Task Handle_Quando_repositorio_retornar_id_positivo_Deve_retornar_true()
        {
            var usuario = CriarUsuario();
            var usuarioAtualizado = CriarUsuario(id: 99);
            var request = new SalvarUsuarioParcialCommand(Login, "Maria da Silva");

            repositorioUsuarioMock
                .Setup(r => r.ObterPorLogin(Login))
                .ReturnsAsync(usuario);
            repositorioUsuarioMock
                .Setup(r => r.Atualizar(usuario))
                .ReturnsAsync(usuarioAtualizado);

            var resultado = await sut.Handle(request, CancellationToken.None);

            Assert.True(resultado);
        }

        [Fact]
        public async Task Handle_Quando_primeira_remocao_de_cache_falhar_Deve_propagar_excecao_e_nao_atualizar()
        {
            var usuario = CriarUsuario();
            var request = new SalvarUsuarioParcialCommand(Login, "Maria da Silva");
            var excecaoEsperada = new InvalidOperationException("Falha ao remover cache");

            repositorioUsuarioMock
                .Setup(r => r.ObterPorLogin(Login))
                .ReturnsAsync(usuario);
            mediatorMock
                .Setup(m => m.Send(
                    It.Is<RemoverCacheCommand>(c =>
                        c.Chave == CacheDistribuidoNomes.Usuario.Parametros(Login)),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(excecaoEsperada);

            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
                () => sut.Handle(request, CancellationToken.None));

            Assert.Same(excecaoEsperada, excecao);
            mediatorMock.Verify(
                m => m.Send(
                    It.Is<RemoverCacheCommand>(c =>
                        c.Chave == CacheDistribuidoNomes.UsuarioLogado.Parametros(Login)),
                    It.IsAny<CancellationToken>()),
                Times.Never);
            repositorioUsuarioMock.Verify(
                r => r.Atualizar(It.IsAny<Usuario>()),
                Times.Never);
        }

        private static Usuario CriarUsuario(long id = 1)
        {
            return new Usuario
            {
                Id = id,
                Login = Login,
                Nome = "Nome anterior",
                NomeSocial = "Nome social anterior"
            };
        }
    }
}
