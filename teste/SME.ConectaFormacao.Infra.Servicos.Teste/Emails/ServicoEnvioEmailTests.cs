using Bogus;
using Elastic.Apm.Api;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Infra.Servicos.Acessos;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Emails;
using SME.ConectaFormacao.Infra.Servicos.Emails.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

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
        public async Task DeveEnviarEmailComSucesso()
        {
            // Arrange
            var emailOrigem = _faker.Internet.Email();
            var destinatario = _faker.Internet.Email();
            var assunto = _faker.Lorem.Sentence();
            var conteudo = _faker.Lorem.Paragraph();

            var mensagem = new MimeMessage();
            mensagem.From.Add(new MailboxAddress("Origem", emailOrigem));
            mensagem.To.Add(new MailboxAddress("Destino", destinatario));
            mensagem.Subject = assunto;
            mensagem.Body = new TextPart("plain")
            {
                Text = conteudo
            };

            _servicoAcessosMock
                .Setup(x => x.ObterConfiguracaoEmail())
                .ReturnsAsync(new AcessosConfiguracaoEmailRetorno
                {
                    Email = emailOrigem,
                    Nome = "Conecta Formação - Não responder",
                    Porta = 587,
                    Senha = _faker.Internet.Password(),
                    Smtp = "smtp.exemplo.com",
                    TLS = true,
                    Usuario = emailOrigem
                });

            // Act
            await _sut.EnviarAsync(mensagem, CancellationToken.None);

            // Assert
            _smtpClientMock.Verify(x => x.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
            _smtpClientMock.Verify(x => x.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _smtpClientMock.Verify(x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()), Times.Once);
            _smtpClientMock.Verify(x => x.DisconnectAsync(true, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeveTentarNovamenteAoFalharConexao()
        {
            // Arrange
            var emailOrigem = _faker.Internet.Email();
            var destinatario = _faker.Internet.Email();
            var assunto = _faker.Lorem.Sentence();
            var conteudo = _faker.Lorem.Paragraph();
            var mensagem = new MimeMessage();
            mensagem.From.Add(new MailboxAddress("Origem", emailOrigem));
            mensagem.To.Add(new MailboxAddress("Destino", destinatario));
            mensagem.Subject = assunto;
            mensagem.Body = new TextPart("plain")
            {
                Text = conteudo
            };
            _servicoAcessosMock
                .Setup(x => x.ObterConfiguracaoEmail())
                .ReturnsAsync(new AcessosConfiguracaoEmailRetorno
                {
                    Email = emailOrigem,
                    Nome = "Conecta Formação - Não responder",
                    Porta = 587,
                    Senha = _faker.Internet.Password(),
                    Smtp = "smtp.exemplo.com",
                    TLS = true,
                    Usuario = emailOrigem
                });
            _smtpClientMock
                .SetupSequence(x => x.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new IOException("Falha de rede simulada 1"))
                .ThrowsAsync(new IOException("Falha de rede simulada 2"))
                .Returns(Task.CompletedTask);

            // Act
            await _sut.EnviarAsync(mensagem, CancellationToken.None);

            // Assert
            _smtpClientMock.Verify(x => x.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
            _smtpClientMock.Verify(x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()), Times.Once);
        }

        [Fact]
        public async Task DeveRetornarExcecaoAoEsgotarTentativas()
        {
            // Arrange
            var emailOrigem = _faker.Internet.Email();
            var destinatario = _faker.Internet.Email();
            var assunto = _faker.Lorem.Sentence();
            var conteudo = _faker.Lorem.Paragraph();
            var mensagem = new MimeMessage();
            mensagem.From.Add(new MailboxAddress("Origem", emailOrigem));
            mensagem.To.Add(new MailboxAddress("Destino", destinatario));
            mensagem.Subject = assunto;
            mensagem.Body = new TextPart("plain")
            {
                Text = conteudo
            };
            _servicoAcessosMock
                .Setup(x => x.ObterConfiguracaoEmail())
                .ReturnsAsync(new AcessosConfiguracaoEmailRetorno
                {
                    Email = emailOrigem,
                    Nome = "Conecta Formação - Não responder",
                    Porta = 587,
                    Senha = _faker.Internet.Password(),
                    Smtp = "smtp.exemplo.com",
                    TLS = true,
                    Usuario = emailOrigem
                });
            _smtpClientMock
                .SetupSequence(x => x.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new IOException("Falha de rede simulada 1"))
                .ThrowsAsync(new SocketException())
                .ThrowsAsync(new IOException("Falha de rede simulada 3"));

            // Act & Assert
            await Assert.ThrowsAnyAsync<Exception>(() => _sut.EnviarAsync(mensagem, CancellationToken.None));
            _smtpClientMock
                .Verify(x => x.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())
                , Times.Exactly(4));
        }
    }
}
