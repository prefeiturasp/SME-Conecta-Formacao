using Bogus;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Infra.Servicos.Acessos;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Emails;
using SME.ConectaFormacao.Infra.Servicos.Emails.Interfaces;

namespace SME.ConectaFormacao.Infra.Servicos.Teste.Emails
{
    public class ServicoEnvioEmailTests
    {
        private readonly Mock<ISmtpClient> _smtpClientMock;
        private readonly Mock<ISmtpClientFactory> _smtpClientFactoryMock;
        private readonly Mock<IServicoAcessos> _servicoAcessosMock;
        private readonly ServicoEnvioEmail _sut;
        private readonly Faker _faker;

        public ServicoEnvioEmailTests()
        {
            var mocker = new AutoMocker();
            _smtpClientMock = new Mock<ISmtpClient>();
            _smtpClientFactoryMock = mocker.GetMock<ISmtpClientFactory>();
            _smtpClientFactoryMock
                .Setup(x => x.Criar())
                .Returns(_smtpClientMock.Object);
            _servicoAcessosMock = mocker.GetMock<IServicoAcessos>();
            _sut = mocker.CreateInstance<ServicoEnvioEmail>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoPrimeiroEnvio_QuandoEnviarComIdempotencia_EntaoDeveEnviarComSucesso()
        {
            // Arrange
            var destinatario = _faker.Internet.Email();
            var assunto = _faker.Lorem.Sentence();
            var chaveIdempotencia = "chave-teste-123";

            var mensagem = new MimeMessage();
            mensagem.From.Add(new MailboxAddress("Origem", _faker.Internet.Email()));
            mensagem.To.Add(new MailboxAddress("Destino", destinatario));
            mensagem.Subject = assunto;
            mensagem.Body = new TextPart("plain") { Text = _faker.Lorem.Paragraph() };

            var configuracao = new AcessosConfiguracaoEmailRetorno
            {
                Smtp = "smtp.teste.com",
                Porta = 587,
                Usuario = "usuario",
                Senha = "senha",
                TLS = true
            };

            _servicoAcessosMock.Setup(x => x.ObterConfiguracaoEmail()).ReturnsAsync(configuracao);

           _smtpClientMock.Setup(x => x.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _smtpClientMock.Setup(x => x.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _smtpClientMock.Setup(x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()))
                .Returns(Task.FromResult(string.Empty));
            _smtpClientMock.Setup(x => x.DisconnectAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _sut.EnviarComIdempotenciaAsync(mensagem, chaveIdempotencia);

            // Assert
            Assert.True(resultado.Enviado);
            Assert.False(resultado.JaEnviado);
            Assert.Null(resultado.MensagemErro);
            Assert.Equal(chaveIdempotencia, resultado.ChaveIdempotencia);
        }

        [Fact]
        public async Task DadoEmailJaEnviadoNaMesmaExecucao_QuandoEnviarComIdempotencia_EntaoDeveRetornarJaEnviado()
        {
            // Arrange
            var destinatario = _faker.Internet.Email();
            var assunto = _faker.Lorem.Sentence();
            var chaveIdempotencia = "chave-duplicada-456";

            var mensagem = new MimeMessage();
            mensagem.From.Add(new MailboxAddress("Origem", _faker.Internet.Email()));
            mensagem.To.Add(new MailboxAddress("Destino", destinatario));
            mensagem.Subject = assunto;
            mensagem.Body = new TextPart("plain") { Text = _faker.Lorem.Paragraph() };

            var configuracao = new AcessosConfiguracaoEmailRetorno
            {
                Smtp = "smtp.teste.com",
                Porta = 587,
                Usuario = "usuario",
                Senha = "senha",
                TLS = true
            };

            _servicoAcessosMock.Setup(x => x.ObterConfiguracaoEmail()).ReturnsAsync(configuracao);

            _smtpClientMock.Setup(x => x.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _smtpClientMock.Setup(x => x.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _smtpClientMock.Setup(x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()))
                .Returns(Task.FromResult(string.Empty));
            _smtpClientMock.Setup(x => x.DisconnectAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act - Primeiro envio
            await _sut.EnviarComIdempotenciaAsync(mensagem, chaveIdempotencia);

            // Act - Segundo envio (duplicata)
            var resultado = await _sut.EnviarComIdempotenciaAsync(mensagem, chaveIdempotencia);

            // Assert
            Assert.False(resultado.Enviado);
            Assert.True(resultado.JaEnviado);
            Assert.Null(resultado.MensagemErro);
            Assert.Equal(chaveIdempotencia, resultado.ChaveIdempotencia);
            // Verifica que o SMTP foi chamado apenas uma vez
            _smtpClientMock.Verify(x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()), Times.Once);
        }

        [Fact]
        public async Task DadoErroNoEnvio_QuandoEnviarComIdempotencia_EntaoDeveRetornarErro()
        {
            // Arrange
            var destinatario = _faker.Internet.Email();
            var assunto = _faker.Lorem.Sentence();
            var chaveIdempotencia = "chave-erro-789";

            var mensagem = new MimeMessage();
            mensagem.From.Add(new MailboxAddress("Origem", _faker.Internet.Email()));
            mensagem.To.Add(new MailboxAddress("Destino", destinatario));
            mensagem.Subject = assunto;
            mensagem.Body = new TextPart("plain") { Text = _faker.Lorem.Paragraph() };

            var configuracao = new AcessosConfiguracaoEmailRetorno
            {
                Smtp = "smtp.teste.com",
                Porta = 587,
                Usuario = "usuario",
                Senha = "senha",
                TLS = true
            };

            _servicoAcessosMock.Setup(x => x.ObterConfiguracaoEmail()).ReturnsAsync(configuracao);

            _smtpClientMock.Setup(x => x.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _smtpClientMock.Setup(x => x.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _smtpClientMock.Setup(x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()))
                .ThrowsAsync(new SmtpCommandException(SmtpErrorCode.MessageNotAccepted, SmtpStatusCode.TransactionFailed, "Erro no servidor SMTP"));

            // Act
            var resultado = await _sut.EnviarComIdempotenciaAsync(mensagem, chaveIdempotencia);

            // Assert
            Assert.False(resultado.Enviado);
            Assert.False(resultado.JaEnviado);
            Assert.NotNull(resultado.MensagemErro);
            Assert.Contains("Erro ao enviar e-mail", resultado.MensagemErro);
        }
    }
}
