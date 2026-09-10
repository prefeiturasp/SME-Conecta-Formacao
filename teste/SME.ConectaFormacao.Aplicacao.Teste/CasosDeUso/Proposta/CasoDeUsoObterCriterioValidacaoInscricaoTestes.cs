using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Proposta
{
    public class CasoDeUsoObterCriterioValidacaoInscricaoTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterCriterioValidacaoInscricao _casoDeUso;
        private readonly Faker _faker;

        public CasoDeUsoObterCriterioValidacaoInscricaoTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _casoDeUso = mocker.CreateInstance<CasoDeUsoObterCriterioValidacaoInscricao>();
            _faker = new Faker();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task DadoOpcaoOutros_QuandoExecutar_EntaoDeveEnviarQueryCorretaERetornarResultado(bool exibirOpcaoOutros)
        {
            // Arrange
            var resultadoEsperado = new List<CriterioValidacaoInscricaoDTO>
            {
                new() { Id = _faker.Random.Int(), Nome = _faker.Random.Word() }
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterCriterioValidacaoInscricaoQuery>(q => q.ExibirOutros == exibirOpcaoOutros), It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _casoDeUso.Executar(exibirOpcaoOutros);

            // Assert
            resultado.Should().BeEquivalentTo(resultadoEsperado);
            _mediatorMock.Verify(m => m.Send(It.Is<ObterCriterioValidacaoInscricaoQuery>(q => q.ExibirOutros == exibirOpcaoOutros), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
