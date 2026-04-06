using Bogus;
using MimeKit;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.Enviar.EnviarEmail;
using SME.ConectaFormacao.Infra.Servicos.Acessos;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Emails.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Email
{
    public class EnviarEmailCommandHandlerTestes
    {
        private readonly Mock<IServicoEnvioEmail> _mockServicoEnvioEmail;
        private readonly Mock<IServicoAcessos> _mockServicoAcessos;
        private readonly EnviarEmailCommandHandler _handler;
        private readonly Faker _faker;

        public EnviarEmailCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _mockServicoEnvioEmail = mocker.GetMock<IServicoEnvioEmail>();
            _mockServicoAcessos = mocker.GetMock<IServicoAcessos>();
            _handler = mocker.CreateInstance<EnviarEmailCommandHandler>();
            _faker = new();
        }

        [Fact]
        public async Task DadoComandoValido_QuandoProcessado_DeveChamarServicoEnvioEmail()
        {
            // Arrange
            var comando = new EnviarEmailCommand(_faker.Person.FullName, _faker.Internet.Email(), "Assunto Teste", "<h1>Mensagem de Teste</h1>");

            var configEmailMock = new AcessosConfiguracaoEmailRetorno
            {
                Nome = "Conecta Formação - Não responder",
                Email = "conectaformacao-nao_responder@sme.prefeitura.sp.gov.br",
                Porta = 587,
                Senha = _faker.Internet.Password(),
                Smtp = "smtp.teste.com",
                TLS = true,
                Usuario = "conectaformacao-nao_responder@sme.prefeitura.sp.gov.br"
            };

            _mockServicoAcessos
                .Setup(x => x.ObterConfiguracaoEmail())
                .ReturnsAsync(configEmailMock);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            Assert.True(resultado);
            _mockServicoEnvioEmail.Verify(x => x.EnviarAsync(It.IsAny<MimeMessage>(), CancellationToken.None), Times.Once);
        }
    }
}
