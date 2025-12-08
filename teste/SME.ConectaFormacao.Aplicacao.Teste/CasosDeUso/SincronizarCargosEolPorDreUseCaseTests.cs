using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.SincronizacaoEOL;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class SincronizarCargosEolPorDreUseCaseTests
    {
        private readonly Mock<IServicoEol> _servicoEolMock;
        private readonly Mock<IRepositorioSincronizador> _repositorioSincronizadorMock;
        private readonly SincronizarCargosEolPorDreUseCase _useCase;

        public SincronizarCargosEolPorDreUseCaseTests()
        {
            var mocker = new AutoMocker();
            _servicoEolMock = mocker.GetMock<IServicoEol>();
            _repositorioSincronizadorMock = mocker.GetMock<IRepositorioSincronizador>();
            _useCase = mocker.CreateInstance<SincronizarCargosEolPorDreUseCase>();
        }

        [Fact]
        public async Task DadoQueParametroEhNulo_QuandoExecutar_EntaoDeveLancarArgumentNullException()
        {
            // Act
            Func<Task> acao = async () => await _useCase.Executar(new() { Mensagem = "" });

            // Assert
            await acao.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task DadoQueExistemCargosParaSincronizar_QuandoExecutar_EntaoDeveProcessarDadosCorretamente()
        {
            // Arrange
            var codigoDre = "DRE1";
            var cargosEolOrigem = new List<CargoEolDto>
            {
                new() { CdCargo = 1, CdRegistroFuncional = "RF1", CodigoUe = "UE1", Sobreposto = false, CodigoDre = codigoDre },
                new() { CdCargo = 2, CdRegistroFuncional = "RF2", CodigoUe = "UE2", Sobreposto = true, CodigoDre = codigoDre }
            };
            _servicoEolMock
                .Setup(s => s.ObterCargosEolPorDreAsync(codigoDre))
                .ReturnsAsync(cargosEolOrigem);
            // Act
            var resultado = await _useCase.Executar(new() { Mensagem = codigoDre });
            // Assert
            resultado.Should().BeTrue();
            _servicoEolMock.Verify(s => s.ObterCargosEolPorDreAsync(codigoDre), Times.Once);
            _repositorioSincronizadorMock.Verify(r => r.SincronizarLoteCargosEolAsync(
                It.Is<List<CargoEol>>(list => list.Count == 2 &&
                                               list.Exists(c => c.CodigoCargo == 1 && c.CodigoRegistroFuncional == "RF1" && c.CodigoUe == "UE1" && c.Sobreposto == false) &&
                                               list.Exists(c => c.CodigoCargo == 2 && c.CodigoRegistroFuncional == "RF2" && c.CodigoUe == "UE2" && c.Sobreposto == true)),
                codigoDre), Times.Once);
        }

        [Fact]
        public async Task DadoQueNaoExistemCargosParaSincronizar_QuandoExecutar_EntaoDeveProcessarDadosCorretamente()
        {
            // Arrange
            var codigoDre = "DRE1";
            _servicoEolMock
                .Setup(s => s.ObterCargosEolPorDreAsync(codigoDre))
                .ReturnsAsync([]);
            // Act
            var resultado = await _useCase.Executar(new() { Mensagem = codigoDre });
            // Assert
            resultado.Should().BeTrue();
            _servicoEolMock.Verify(s => s.ObterCargosEolPorDreAsync(codigoDre), Times.Once);
            _repositorioSincronizadorMock.Verify(r => r.SincronizarLoteCargosEolAsync(
                It.Is<List<CargoEol>>(list => list.Count == 0),
                codigoDre), Times.Once);
        }

        [Fact]
        public async Task DadoQueServicoEolRetornaNulo_QuandoExecutar_EntaoDeveProcessarDadosCorretamente()
        {
            // Arrange
            var codigoDre = "DRE1";
            _servicoEolMock
                .Setup(s => s.ObterCargosEolPorDreAsync(codigoDre))
                .ReturnsAsync((List<CargoEolDto>?)null);
            // Act
            var resultado = await _useCase.Executar(new() { Mensagem = codigoDre });
            // Assert
            resultado.Should().BeTrue();
            _servicoEolMock.Verify(s => s.ObterCargosEolPorDreAsync(codigoDre), Times.Once);
            _repositorioSincronizadorMock.Verify(r => r.SincronizarLoteCargosEolAsync(
                It.Is<List<CargoEol>>(list => list.Count == 0),
                codigoDre), Times.Once);
        }
    }
}