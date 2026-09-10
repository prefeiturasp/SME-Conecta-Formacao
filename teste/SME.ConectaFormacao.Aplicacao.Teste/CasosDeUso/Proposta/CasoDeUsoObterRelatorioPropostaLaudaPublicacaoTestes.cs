using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Proposta
{
    public class CasoDeUsoObterRelatorioPropostaLaudaPublicacaoTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterRelatorioPropostaLaudaPublicacao _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterRelatorioPropostaLaudaPublicacaoTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoObterRelatorioPropostaLaudaPublicacao>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoPropostaIdValido_QuandoExecutar_EntaoDeveEnviarQueryERetornarRelatorioEsperado()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 10000);
            var relatorioEsperado = _faker.Internet.Url();

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterRelatorioProspostaLaudaPublicacaoQuery>(q => q.PropostaId == propostaId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(relatorioEsperado);

            // Act
            var resultado = await _sut.Executar(propostaId);

            // Assert
            resultado.Should().Be(relatorioEsperado);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterRelatorioProspostaLaudaPublicacaoQuery>(q => q.PropostaId == propostaId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task DadoQueryRetornaVazioOuNulo_QuandoExecutar_EntaoDeveRetornarVazioOuNulo(string? retornoQuery)
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 10000);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterRelatorioProspostaLaudaPublicacaoQuery>(q => q.PropostaId == propostaId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(retornoQuery!);

            // Act
            var resultado = await _sut.Executar(propostaId);

            // Assert
            resultado.Should().Be(retornoQuery);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterRelatorioProspostaLaudaPublicacaoQuery>(q => q.PropostaId == propostaId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoErroAoEnviarQuery_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 10000);
            var mensagemErro = _faker.Lorem.Sentence();

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterRelatorioProspostaLaudaPublicacaoQuery>(q => q.PropostaId == propostaId), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = async () => await _sut.Executar(propostaId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage(mensagemErro);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterRelatorioProspostaLaudaPublicacaoQuery>(q => q.PropostaId == propostaId),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
