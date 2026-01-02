using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoTurmaPossuiCodafListaPresencaTests
    {
        private readonly Mock<IRepositorioCodafListaPresenca> repositorioCodafListaPresencaMock;
        private readonly CasoDeUsoTurmaPossuiCodafListaPresenca casoDeUsoTurmaPossuiCodafListaPresenca;
        public CasoDeUsoTurmaPossuiCodafListaPresencaTests()
        {
            var mocker = new AutoMocker();
            repositorioCodafListaPresencaMock = mocker.GetMock<IRepositorioCodafListaPresenca>();
            casoDeUsoTurmaPossuiCodafListaPresenca = mocker.CreateInstance<CasoDeUsoTurmaPossuiCodafListaPresenca>();
        }

        [Fact]
        public async Task DadoUmPropostaTurmaId_QuandoExecutar_DeveRetornarSePossuiListaPresenca()
        {
            // Arrange
            long propostaTurmaId = 1;
            long listaPresencaId = 0;
            repositorioCodafListaPresencaMock
                .Setup(r => r.TurmaJaTemListaDePresencaAsync(propostaTurmaId, listaPresencaId))
                .ReturnsAsync(true);
            // Act
            var resultado = await casoDeUsoTurmaPossuiCodafListaPresenca.ExecutarAsync(propostaTurmaId, listaPresencaId);
            // Assert
            resultado.Sucesso.Should().BeTrue();
            repositorioCodafListaPresencaMock.Verify(r => r.TurmaJaTemListaDePresencaAsync(propostaTurmaId, listaPresencaId), Times.Once);
        }
    }
}
