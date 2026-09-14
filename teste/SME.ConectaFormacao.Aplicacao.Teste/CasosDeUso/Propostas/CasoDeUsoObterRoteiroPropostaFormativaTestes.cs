using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Propostas
{
    public class CasoDeUsoObterRoteiroPropostaFormativaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterRoteiroPropostaFormativa _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterRoteiroPropostaFormativaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoObterRoteiroPropostaFormativa>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoRoteiroExistente_QuandoExecutar_EntaoDeveEnviarQueryERetornarRoteiroEsperado()
        {
            // Arrange
            var roteiroEsperado = new RoteiroPropostaFormativaDTO
            {
                Id = _faker.Random.Long(1, 100),
                Descricao = _faker.Lorem.Sentence()
            };

            _mediatorMock
                .Setup(m => m.Send(ObterRoteiroPropostaFormativaQuery.Instancia, It.IsAny<CancellationToken>()))
                .ReturnsAsync(roteiroEsperado);

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeSameAs(roteiroEsperado);
            _mediatorMock.Verify(m => m.Send(ObterRoteiroPropostaFormativaQuery.Instancia, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoRoteiroNaoEncontrado_QuandoExecutar_EntaoDeveRetornarNulo()
        {
            // Arrange
            _mediatorMock
                .Setup(m => m.Send(ObterRoteiroPropostaFormativaQuery.Instancia, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RoteiroPropostaFormativaDTO)null!);

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().BeNull();
            _mediatorMock.Verify(m => m.Send(ObterRoteiroPropostaFormativaQuery.Instancia, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoFalhaNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var mensagemErro = "Falha ao consultar roteiro da proposta formativa";

            _mediatorMock
                .Setup(m => m.Send(ObterRoteiroPropostaFormativaQuery.Instancia, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = async () => await _sut.Executar();

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage(mensagemErro);

            _mediatorMock.Verify(m => m.Send(
                ObterRoteiroPropostaFormativaQuery.Instancia,
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
