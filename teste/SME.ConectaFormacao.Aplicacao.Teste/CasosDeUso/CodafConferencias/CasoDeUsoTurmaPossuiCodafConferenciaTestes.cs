using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafConferencias;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.CodafConferencias
{
    public class CasoDeUsoTurmaPossuiCodafConferenciaTestes
    {
        private readonly Mock<IRepositorioCodafConferencia> _repositorioMock;
        private readonly CasoDeUsoTurmaPossuiCodafConferencia _sut;

        public CasoDeUsoTurmaPossuiCodafConferenciaTestes()
        {
            var mocker = new AutoMocker();
            _repositorioMock = mocker.GetMock<IRepositorioCodafConferencia>();
            _sut = mocker.CreateInstance<CasoDeUsoTurmaPossuiCodafConferencia>();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task DadoPropostaTurmaId_QuandoExecutar_EntaoDeveRetornarResultadoDoRepositorio(bool possui)
        {
            // Arrange
            _repositorioMock.Setup(r => r.TurmaJaTemCodafAsync(10, 5)).ReturnsAsync(possui);

            // Act
            var resultado = await _sut.ExecutarAsync(10, 5);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().Be(possui);
        }
    }
}
