using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Propostas
{
    public class CasoDeUsoObterNumeroHomologacaoPropostaTestes
    {
        private readonly Mock<IRepositorioProposta> _repositorioPropostaMock;
        private readonly CasoDeUsoObterNumeroHomologacaoProposta _sut;

        public CasoDeUsoObterNumeroHomologacaoPropostaTestes()
        {
            var mocker = new AutoMocker();
            _repositorioPropostaMock = mocker.GetMock<IRepositorioProposta>();
            _sut = mocker.CreateInstance<CasoDeUsoObterNumeroHomologacaoProposta>();
        }

        [Fact]
        public async Task DadoPropostaIdValidoComHomologacao_QuandoExecutarAsync_EntaoDeveRetornarNumeroHomologacao()
        {
            // Arrange
            const long propostaId = 12345;
            const long numeroHomologacaoEsperado = 987654;

            _repositorioPropostaMock
                .Setup(r => r.ObterNumeroHomologacaoPorPropostaIdAsync(propostaId))
                .ReturnsAsync(numeroHomologacaoEsperado);

            // Act
            var resultado = await _sut.ExecutarAsync(propostaId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados!.NumeroHomologacao.Should().Be(numeroHomologacaoEsperado);

            _repositorioPropostaMock.Verify(r => r.ObterNumeroHomologacaoPorPropostaIdAsync(propostaId), Times.Once);
        }

        [Fact]
        public async Task DadoPropostaIdValidoSemHomologacao_QuandoExecutarAsync_EntaoDeveRetornarNumeroHomologacaoNulo()
        {
            // Arrange
            const long propostaId = 12345;

            _repositorioPropostaMock
                .Setup(r => r.ObterNumeroHomologacaoPorPropostaIdAsync(propostaId))
                .ReturnsAsync((long?)null);

            // Act
            var resultado = await _sut.ExecutarAsync(propostaId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados!.NumeroHomologacao.Should().BeNull();

            _repositorioPropostaMock.Verify(r => r.ObterNumeroHomologacaoPorPropostaIdAsync(propostaId), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public async Task DadoPropostaIdInvalidoMenorOuIgualAZero_QuandoExecutarAsync_EntaoDeveRetornarNumeroHomologacaoNuloSemConsultarRepositorio(long propostaIdInvalido)
        {
            // Act
            var resultado = await _sut.ExecutarAsync(propostaIdInvalido);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados!.NumeroHomologacao.Should().BeNull();

            _repositorioPropostaMock.Verify(r => r.ObterNumeroHomologacaoPorPropostaIdAsync(It.IsAny<long>()), Times.Never);
        }
    }
}
