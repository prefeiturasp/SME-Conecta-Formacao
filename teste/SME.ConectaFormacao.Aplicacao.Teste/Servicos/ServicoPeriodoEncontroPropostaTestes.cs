using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Servicos;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Teste.Servicos
{
    [ExcludeFromCodeCoverage]
    public class ServicoPeriodoEncontroPropostaTestes
    {
        private readonly AutoMocker _mocker;
        private readonly ServicoPeriodoEncontroProposta _sut;

        public ServicoPeriodoEncontroPropostaTestes()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<ServicoPeriodoEncontroProposta>();
        }

        [Fact]
        public async Task DadoEncontrosApenasComDatasDeInicio_QuandoObterPeriodo_EntaoRetornaFormatadoCorretamente()
        {
            // Arrange
            var encontros = new List<PropostaEncontro>
            {
                new() { 
                    Datas =
                    [
                        new () { DataInicio = new (2025, 01, 10, 0, 0, 0, DateTimeKind.Unspecified) },
                        new () { DataInicio = new (2025, 01, 15, 0, 0, 0, DateTimeKind.Unspecified) }
                    ]
                }
            };

            _mocker.GetMock<ICacheDistribuido>()
                .Setup(m => m.ObterAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<PropostaEncontro>>>>()))
                .ReturnsAsync(encontros);

            // Act
            var resultado = await _sut.ObterPeriodoEncontrosTurmaAsync(1);

            // Assert
            resultado.Should().Be(" 10/01/2025 até 15/01/2025");
        }

        [Fact]
        public async Task DadoEncontrosComDataFim_QuandoObterPeriodo_EntaoRetornaFormatadoAteMaiorDataFim()
        {
            // Arrange
            var encontros = new List<PropostaEncontro>
            {
                new() {
                    Datas =
                    [
                        new () { DataInicio = new (2025, 01, 10, 0, 0, 0, DateTimeKind.Unspecified) },
                        new () { DataInicio = new (2025, 01, 20, 0, 0, 0, DateTimeKind.Unspecified) }
                    ]
                }
            };

            _mocker.GetMock<ICacheDistribuido>()
                .Setup(m => m.ObterAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<PropostaEncontro>>>>()))
                .ReturnsAsync(encontros);

            // Act
            var resultado = await _sut.ObterPeriodoEncontrosTurmaAsync(1);

            // Assert
            resultado.Should().Be(" 10/01/2025 até 20/01/2025");
        }

        [Fact]
        public async Task DadoApenasUmEncontroSemDataFim_QuandoObterPeriodo_EntaoRetornaApenasDataInicio()
        {
            // Arrange
            var encontros = new List<PropostaEncontro>
            {
                new() {
                    Datas =
                    [
                        new () { DataInicio = new (2025, 01, 10, 0, 0, 0, DateTimeKind.Unspecified) },
                    ]
                }
            };

            _mocker.GetMock<ICacheDistribuido>()
                .Setup(m => m.ObterAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<PropostaEncontro>>>>()))
                .ReturnsAsync(encontros);

            // Act
            var resultado = await _sut.ObterPeriodoEncontrosTurmaAsync(1);

            // Assert
            resultado.Should().Be(" 10/01/2025");
        }
    }
}
