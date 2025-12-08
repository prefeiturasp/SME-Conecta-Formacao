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
    public class SincronizarFuncaoAtividadeEolPorDreUseCaseTests
    {
        private readonly Mock<IServicoEol> _servicoEolMock;
        private readonly Mock<IRepositorioSincronizador> _repositorioSincronizadorMock;
        private readonly SincronizarFuncaoAtividadeEolPorDreUseCase _useCase;

        public SincronizarFuncaoAtividadeEolPorDreUseCaseTests()
        {
            var mocker = new AutoMocker();
            _servicoEolMock = mocker.GetMock<IServicoEol>();
            _repositorioSincronizadorMock = mocker.GetMock<IRepositorioSincronizador>();
            _useCase = mocker.CreateInstance<SincronizarFuncaoAtividadeEolPorDreUseCase>();
        }

        [Fact]
        public async Task DadoQueParametroEhNulo_QuandoExecutar_EntaoDeveLancarArgumentNullException()
        {
            Func<Task> acao = async () => await _useCase.Executar(new() { Mensagem = "" });

            await acao.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task DadoQueExistemFuncoesAtividadesParaSincronizar_QuandoExecutar_EntaoDeveProcessarDadosCorretamente()
        {
            var codigoDre = "DRE1";

            var funcoesAtividadesOrigem = new List<FuncaoAtividadeDto>
            {
                new() { CdRegistroFuncional = "RF1", CdTipoFuncao = "1", CdDre = "DRE1", CdUe = "UE1" },
                new() { CdRegistroFuncional = "RF2", CdTipoFuncao = "2", CdDre = "DRE1", CdUe = "UE2" }
            };

            _servicoEolMock
                .Setup(s => s.ObterFuncaoAtividadeEolPorDre(codigoDre, It.IsAny<DateTime?>()))
                .ReturnsAsync(funcoesAtividadesOrigem);

            var resultado = await _useCase.Executar(new() { Mensagem = codigoDre });

            resultado.Should().BeTrue();
            _servicoEolMock.Verify(s => s.ObterFuncaoAtividadeEolPorDre(codigoDre, It.IsAny<DateTime?>()), Times.Once);

            _repositorioSincronizadorMock.Verify(r => r.SincronizarLoteFuncaoAtividadeEolAsync(
                It.Is<List<FuncaoAtividadeServidorEol>>(list =>
                    list.Count == 2 &&
                    list.Exists(f => f.CdRegistroFuncional == "RF1" && f.CdTipoFuncao == 1 && f.CdDre == "DRE1" && f.CdUe == "UE1") &&
                    list.Exists(f => f.CdRegistroFuncional == "RF2" && f.CdTipoFuncao == 2 && f.CdDre == "DRE1" && f.CdUe == "UE2")
                ),
                codigoDre), Times.Once);
        }

        [Fact]
        public async Task DadoQueNaoExistemFuncoesAtividadesParaSincronizar_QuandoExecutar_EntaoDeveProcessarDadosCorretamente()
        {
            var codigoDre = "DRE1";

            _servicoEolMock
                .Setup(s => s.ObterFuncaoAtividadeEolPorDre(codigoDre, It.IsAny<DateTime?>()))
                .ReturnsAsync(new List<FuncaoAtividadeDto>());

            var resultado = await _useCase.Executar(new() { Mensagem = codigoDre });

            resultado.Should().BeTrue();
            _servicoEolMock.Verify(s => s.ObterFuncaoAtividadeEolPorDre(codigoDre, It.IsAny<DateTime?>()), Times.Once);

            _repositorioSincronizadorMock.Verify(r => r.SincronizarLoteFuncaoAtividadeEolAsync(
                It.Is<List<FuncaoAtividadeServidorEol>>(list => list.Count == 0),
                codigoDre), Times.Once);
        }

        [Fact]
        public async Task DadoQueServicoEolRetornaNulo_QuandoExecutar_EntaoDeveProcessarDadosCorretamente()
        {
            var codigoDre = "DRE1";

            _servicoEolMock
                .Setup(s => s.ObterFuncaoAtividadeEolPorDre(codigoDre, It.IsAny<DateTime?>()))
                .ReturnsAsync((List<FuncaoAtividadeDto>?)null);

            var resultado = await _useCase.Executar(new() { Mensagem = codigoDre });

            resultado.Should().BeTrue();
            _servicoEolMock.Verify(s => s.ObterFuncaoAtividadeEolPorDre(codigoDre, It.IsAny<DateTime?>()), Times.Once);

            _repositorioSincronizadorMock.Verify(r => r.SincronizarLoteFuncaoAtividadeEolAsync(
                It.Is<List<FuncaoAtividadeServidorEol>>(list => list.Count == 0),
                codigoDre), Times.Once);
        }
    }
}
