using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Hosting;
using MimeKit;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.ServicoAcessos.EnviarEmailAdminSolicitacaoResetSenha;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Acessos;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Emails.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Usuarios
{
    public class EnviarEmailAdminSolicitacaoResetSenhaCommandHandlerTestes
    {
        private readonly AutoMocker _mocker;
        private readonly EnviarEmailAdminSolicitacaoResetSenhaCommandHandler _sut;
        private readonly Faker _faker;

        private readonly Mock<IRepositorioUsuario> _repositorioUsuario;
        private readonly Mock<IServicoEnvioEmail> _servicoEnvioEmail;   
        private readonly Mock<IServicoAcessos> _servicoAcessos;
        private readonly Mock<IHostEnvironment> _hostEnvironment;

        public EnviarEmailAdminSolicitacaoResetSenhaCommandHandlerTestes()
        {
            _mocker = new AutoMocker();
            _repositorioUsuario = _mocker.GetMock<IRepositorioUsuario>();
            _servicoEnvioEmail = _mocker.GetMock<IServicoEnvioEmail>();
            _servicoAcessos = _mocker.GetMock<IServicoAcessos>();
            _hostEnvironment = _mocker.GetMock<IHostEnvironment>();
            _sut = _mocker.CreateInstance<EnviarEmailAdminSolicitacaoResetSenhaCommandHandler>();
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task DadoUsuarioInexistente_QuandoHandle_DeveLancarExcecaoENaoEnviarEmail()
        {
            // Arrange
            var comando = GerarComando();

            _repositorioUsuario
                .Setup(r => r.ObterPorLogin(comando.Login))
                .ReturnsAsync((Dominio.Entidades.Usuario)null!);

            // Act
            var act = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<NegocioException>()
                .WithMessage(MensagemNegocio.LOGIN_NAO_ENCONTRADO);

            _servicoEnvioEmail.Verify(e => e.EnviarAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoAmbienteDesenvolvimento_QuandoHandle_NaoDeveEnviarEmail()
        {
            // Arrange
            var comando = GerarComando();
            var usuario = GerarUsuario();

            var configEmail = new AcessosConfiguracaoEmailRetorno { Email = "teste@dominio.com" };

            _repositorioUsuario
                .Setup(r => r.ObterPorLogin(comando.Login))
                .ReturnsAsync(usuario);

            _servicoAcessos
                .Setup(s => s.ObterConfiguracaoEmail())
                .ReturnsAsync(configEmail);

            _hostEnvironment
                .SetupGet(h => h.EnvironmentName)
                .Returns(Environments.Development);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();

            _servicoEnvioEmail.Verify(e => e.EnviarAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoAmbienteProducao_QuandoHandle_DeveEnviarEmailComDadosCorretos()
        {
            // Arrange
            var comando = GerarComando();
            var usuario = GerarUsuario();

            var configEmail = new AcessosConfiguracaoEmailRetorno { Email = "teste@dominio.com" };

            MimeMessage emailCapturado = null!;

            _repositorioUsuario
                .Setup(r => r.ObterPorLogin(comando.Login))
                .ReturnsAsync(usuario);

            _servicoAcessos
                .Setup(s => s.ObterConfiguracaoEmail())
                .ReturnsAsync(configEmail);

            _hostEnvironment
                .SetupGet(h => h.EnvironmentName)
                .Returns(Environments.Production);

            _servicoEnvioEmail
                .Setup(e => e.EnviarAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()))
                .Callback<MimeMessage, CancellationToken>((msg, _) => emailCapturado = msg)
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();

            _servicoEnvioEmail.Verify(e => e.EnviarAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()), Times.Once);

            emailCapturado.Should().NotBeNull();
            emailCapturado.Subject.Should().Be("SOLICITAÇÃO DE RESET DE SENHA");

            var corpo = (emailCapturado.Body as TextPart)?.Text;
            corpo.Should().NotBeNull();
            corpo.Should().Contain(usuario.Nome);
            corpo.Should().Contain(usuario.Email);
        }

        #region Helpers

        private EnviarEmailAdminSolicitacaoResetSenhaCommand GerarComando()
        {
            return new EnviarEmailAdminSolicitacaoResetSenhaCommand(_faker.Internet.UserName());
        }

        private Dominio.Entidades.Usuario GerarUsuario()
        {
            return new Dominio.Entidades.Usuario
            {
                Nome = _faker.Name.FullName(),
                Email = _faker.Internet.Email(),
                EmailEducacional = _faker.Internet.Email(),
                Login = _faker.Random.AlphaNumeric(8)
            };
        }

        #endregion
    }
}