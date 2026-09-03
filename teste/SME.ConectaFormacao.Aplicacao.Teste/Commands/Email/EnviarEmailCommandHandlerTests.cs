using Bogus;
using MimeKit;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.Enviar.EnviarEmail;
using SME.ConectaFormacao.Dominio.Dtos;
using SME.ConectaFormacao.Infra.Servicos.Acessos;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Emails.Interfaces;

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
        public async Task DadoComandoValido_QuandoProcessado_DeveChamarServicoEnvioEmailComIdempotencia()
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

            _mockServicoEnvioEmail
                .Setup(x => x.EnviarComIdempotenciaAsync(
                    It.IsAny<MimeMessage>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultadoEnvioEmail.Sucesso(_faker.Random.AlphaNumeric(64)));

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            Assert.True(resultado);
            _mockServicoEnvioEmail.Verify(x => x.EnviarComIdempotenciaAsync(
                It.IsAny<MimeMessage>(),
                It.IsAny<string>(),
                CancellationToken.None), Times.Once);
        }
    }
}
