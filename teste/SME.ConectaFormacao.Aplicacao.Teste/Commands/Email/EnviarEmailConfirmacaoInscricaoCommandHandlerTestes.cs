using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Dominio.Interfaces;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Email
{
    public class EnviarEmailConfirmacaoInscricaoCommandHandlerTestes
    {
        private readonly Mock<IRepositorioInscricao> _repositorioInscricaoMock;
        private readonly Mock<IServicoTemplateEmail> _servicoTemplateEmailMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly EnviarEmailConfirmacaoInscricaoCommandHandler _handler;
        private readonly Faker _faker;

        public EnviarEmailConfirmacaoInscricaoCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _repositorioInscricaoMock = mocker.GetMock<IRepositorioInscricao>();
            _servicoTemplateEmailMock = mocker.GetMock<IServicoTemplateEmail>();
            _mediatorMock = mocker.GetMock<IMediator>();
            _faker = new();
            _handler = mocker.CreateInstance<EnviarEmailConfirmacaoInscricaoCommandHandler>();
        }

        [Fact]
        public async Task DadoDadosParaEmailNull_QuandoExecutarHandler_DeveRetornarFalse()
        {
            // Arrange
            var comando = new EnviarEmailConfirmacaoInscricaoCommand(_faker.Random.Long());

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeFalse();
            _mediatorMock.Verify(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoDadosParaEmailVazio_QuandoExecutarHandler_DeveRetornarFalse()
        {
            // Arrange
            var comando = new EnviarEmailConfirmacaoInscricaoCommand(_faker.Random.Long());
            _repositorioInscricaoMock
                .Setup(r => r.ObterDadosInscricaoPorInscricaoId(It.IsAny<long>()))
                .ReturnsAsync([]);
            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);
            // Assert
            resultado.Should().BeFalse();
            _mediatorMock.Verify(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoDadosParaEmailValidos_QuandoExecutarHandler_DeveEnviarEmailERetornarTrue()
        {
            // Arrange
            var comando = new EnviarEmailConfirmacaoInscricaoCommand(_faker.Random.Long());
            var dadosParaEmail = new List<InscricaoDadosEmailConfirmacao>
            {
                new()
                {
                    Email = _faker.Internet.Email(),
                    NomeDestinatario = _faker.Name.FullName(),
                    NomeFormacao = _faker.Lorem.Word()
                }
            };
            _repositorioInscricaoMock
                .Setup(r => r.ObterDadosInscricaoPorInscricaoId(It.IsAny<long>()))
                .ReturnsAsync(dadosParaEmail);
            _servicoTemplateEmailMock
                .Setup(s => s.ObterHtmlInscricaoConfirmadaAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("<html>Conteúdo do Email</html>");

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
