using Bogus;
using FluentAssertions;
using MimeKit;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.Enviar.EnviarEmail;
using SME.ConectaFormacao.Dominio.Dtos;
using SME.ConectaFormacao.Infra.Servicos.Acessos;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Emails.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Comandos.Email
{
    public class EnviarEmailCommandHandlerTestes
    {
        private readonly AutoMocker _mocker;
        private readonly EnviarEmailCommandHandler _sut;
        private readonly Faker _faker;

        public EnviarEmailCommandHandlerTestes()
        {
            _mocker = new AutoMocker();
            _faker = new Faker("pt_BR");

            var configuracaoEmail = new AcessosConfiguracaoEmailRetorno
            {
                Nome = _faker.Person.FullName,
                Email = _faker.Internet.Email(),
                Smtp = "smtp.teste.com",
                Porta = 587,
                Usuario = "usuario",
                Senha = "senha",
                TLS = true
            };

            _mocker.GetMock<IServicoAcessos>()
                .Setup(x => x.ObterConfiguracaoEmail())
                .ReturnsAsync(configuracaoEmail);

            _sut = _mocker.CreateInstance<EnviarEmailCommandHandler>();
        }

        [Fact]
        public async Task DadoPrimeiroEnvio_QuandoEnviar_EntaoDeveEnviarComSucessoERetornarTrue()
        {
            // Arrange
            var command = CriarCommand();

            _mocker.GetMock<IServicoEnvioEmail>()
                .Setup(x => x.EnviarComIdempotenciaAsync(
                    It.IsAny<MimeMessage>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultadoEnvioEmail.Sucesso(_faker.Random.AlphaNumeric(64)));

            // Act
            var resultado = await _sut.Handle(command, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();

            _mocker.GetMock<IServicoEnvioEmail>()
                .Verify(x => x.EnviarComIdempotenciaAsync(
                    It.Is<MimeMessage>(m =>
                        m.To.Mailboxes.Any(mb => mb.Address == command.EmailDestinatario) &&
                        m.Subject == command.Assunto),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Fact]
        public async Task DadoEmailJaEnviado_QuandoEnviar_EntaoDeveRetornarTrueSemEnviarNovamente()
        {
            // Arrange
            var command = CriarCommand();

            _mocker.GetMock<IServicoEnvioEmail>()
                .Setup(x => x.EnviarComIdempotenciaAsync(
                    It.IsAny<MimeMessage>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultadoEnvioEmail.JaEnviadoAnteriormente(_faker.Random.AlphaNumeric(64)));

            // Act
            var resultado = await _sut.Handle(command, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();

            _mocker.GetMock<IServicoEnvioEmail>()
                .Verify(x => x.EnviarComIdempotenciaAsync(
                    It.IsAny<MimeMessage>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Fact]
        public async Task DadoErroNoEnvio_QuandoEnviar_EntaoDeveRetornarFalse()
        {
            // Arrange
            var command = CriarCommand();

            _mocker.GetMock<IServicoEnvioEmail>()
                .Setup(x => x.EnviarComIdempotenciaAsync(
                    It.IsAny<MimeMessage>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultadoEnvioEmail.Erro(
                    _faker.Random.AlphaNumeric(64),
                    _faker.Lorem.Sentence()));

            // Act
            var resultado = await _sut.Handle(command, CancellationToken.None);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task DadoComando_QuandoEnviar_EntaoDeveMontarMensagemComDadosCorretos()
        {
            // Arrange
            var command = CriarCommand();
            MimeMessage? mensagemCapturada = null;

            _mocker.GetMock<IServicoEnvioEmail>()
                .Setup(x => x.EnviarComIdempotenciaAsync(
                    It.IsAny<MimeMessage>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Callback<MimeMessage, string, CancellationToken>((msg, _, _) => mensagemCapturada = msg)
                .ReturnsAsync(ResultadoEnvioEmail.Sucesso(_faker.Random.AlphaNumeric(64)));

            // Act
            await _sut.Handle(command, CancellationToken.None);

            // Assert
            mensagemCapturada.Should().NotBeNull();
            mensagemCapturada!.Subject.Should().Be(command.Assunto);
            mensagemCapturada.To.Mailboxes.Should().ContainSingle(mb =>
                mb.Address == command.EmailDestinatario &&
                mb.Name == command.NomeDestinatario);
            mensagemCapturada.HtmlBody.Should().Be(command.MensagemHtml);
        }

        [Fact]
        public async Task DadoComando_QuandoEnviar_EntaoDeveGerarChaveIdempotenciaBaseadaEmEmailEAssunto()
        {
            // Arrange
            var command = CriarCommand();
            string? chaveCapturada = null;

            _mocker.GetMock<IServicoEnvioEmail>()
                .Setup(x => x.EnviarComIdempotenciaAsync(
                    It.IsAny<MimeMessage>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Callback<MimeMessage, string, CancellationToken>((_, chave, _) => chaveCapturada = chave)
                .ReturnsAsync(ResultadoEnvioEmail.Sucesso(_faker.Random.AlphaNumeric(64)));

            // Act
            await _sut.Handle(command, CancellationToken.None);

            // Assert
            chaveCapturada.Should().NotBeNullOrWhiteSpace();
            chaveCapturada.Should().HaveLength(64); // SHA256 em hexadecimal
            chaveCapturada.Should().MatchRegex("^[a-f0-9]+$");
        }

        [Fact]
        public async Task DadoMesmoEmailEAssunto_QuandoEnviarDuasVezes_EntaoDeveGerarMesmaChaveIdempotencia()
        {
            // Arrange
            var command = CriarCommand();
            var chavesCapturadas = new List<string>();

            _mocker.GetMock<IServicoEnvioEmail>()
                .Setup(x => x.EnviarComIdempotenciaAsync(
                    It.IsAny<MimeMessage>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Callback<MimeMessage, string, CancellationToken>((_, chave, _) => chavesCapturadas.Add(chave))
                .ReturnsAsync(ResultadoEnvioEmail.Sucesso(_faker.Random.AlphaNumeric(64)));

            // Act
            await _sut.Handle(command, CancellationToken.None);
            await _sut.Handle(command, CancellationToken.None);

            // Assert
            chavesCapturadas.Should().HaveCount(2);
            chavesCapturadas[0].Should().Be(chavesCapturadas[1]);
        }

        [Fact]
        public async Task DadoConfiguracaoEmail_QuandoEnviar_EntaoDeveUsarRemetenteConfigurado()
        {
            // Arrange
            var command = CriarCommand();
            var configuracaoEmail = new AcessosConfiguracaoEmailRetorno
            {
                Nome = _faker.Person.FullName,
                Email = _faker.Internet.Email(),
                Smtp = "smtp.teste.com",
                Porta = 587,
                Usuario = "usuario",
                Senha = "senha",
                TLS = true
            };

            _mocker.GetMock<IServicoAcessos>()
                .Setup(x => x.ObterConfiguracaoEmail())
                .ReturnsAsync(configuracaoEmail);

            MimeMessage? mensagemCapturada = null;

            _mocker.GetMock<IServicoEnvioEmail>()
                .Setup(x => x.EnviarComIdempotenciaAsync(
                    It.IsAny<MimeMessage>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Callback<MimeMessage, string, CancellationToken>((msg, _, _) => mensagemCapturada = msg)
                .ReturnsAsync(ResultadoEnvioEmail.Sucesso(_faker.Random.AlphaNumeric(64)));

            // Act
            await _sut.Handle(command, CancellationToken.None);

            // Assert
            mensagemCapturada.Should().NotBeNull();
            mensagemCapturada!.From.Mailboxes.Should().ContainSingle(mb =>
                mb.Address == configuracaoEmail.Email &&
                mb.Name == configuracaoEmail.Nome);
        }

        private EnviarEmailCommand CriarCommand()
        {
            return new EnviarEmailCommand(
                _faker.Person.FullName,
                _faker.Internet.Email(),
                _faker.Lorem.Sentence(),
                $"<html><body><p>{_faker.Lorem.Paragraph()}</p></body></html>");
        }
    }
}
