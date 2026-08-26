using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora;
using SME.ConectaFormacao.Webapi.Controllers;
using System.Net;

namespace SME.ConectaFormacao.Webapi.Teste
{
    public class AreaPromotoraControllerTestes
    {
        private readonly Mock<ICasoDeUsoObterTiposAreaPromotora> _mockObterTipos;
        private readonly Mock<ICasoDeUsoObterAreaPromotoraPaginada> _mockObterPaginada;
        private readonly Mock<ICasoDeUsoObterAreaPromotoraPorId> _mockObterPorId;
        private readonly Mock<ICasoDeUsoObterAreaPromotoraLista> _mockObterLista;
        private readonly Mock<ICasoDeUsoObterAreaPromotoraListaRedeParceria> _mockObterListaRede;
        private readonly Mock<ICasoDeUsoInserirAreaPromotora> _mockInserir;
        private readonly Mock<ICasoDeUsoAlterarAreaPromotora> _mockAlterar;
        private readonly Mock<ICasoDeUsoRemoverAreaPromotora> _mockRemover;
        private readonly AreaPromotoraController _sut;

        public AreaPromotoraControllerTestes()
        {
            var mocker = new AutoMocker();
            _mockObterTipos = mocker.GetMock<ICasoDeUsoObterTiposAreaPromotora>();
            _mockObterPaginada = mocker.GetMock<ICasoDeUsoObterAreaPromotoraPaginada>();
            _mockObterPorId = mocker.GetMock<ICasoDeUsoObterAreaPromotoraPorId>();
            _mockObterLista = mocker.GetMock<ICasoDeUsoObterAreaPromotoraLista>();
            _mockObterListaRede = mocker.GetMock<ICasoDeUsoObterAreaPromotoraListaRedeParceria>();
            _mockInserir = mocker.GetMock<ICasoDeUsoInserirAreaPromotora>();
            _mockAlterar = mocker.GetMock<ICasoDeUsoAlterarAreaPromotora>();
            _mockRemover = mocker.GetMock<ICasoDeUsoRemoverAreaPromotora>();

            _sut = mocker.CreateInstance<AreaPromotoraController>();
        }

        [Fact]
        public async Task DadoRequestValido_QuandoObterTiposAreaPromotora_EntaoRetornaLista()
        {
            // Arrange
            var retorno = new List<AreaPromotoraTipoDTO>();
            _mockObterTipos.Setup(m => m.Executar()).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterTiposAreaPromotora(_mockObterTipos.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
        }

        [Fact]
        public async Task DadoFiltroValido_QuandoObterAreaPromotoraPaginada_EntaoRetornaPaginacao()
        {
            // Arrange
            var filtro = new AreaPromotoraFiltrosDTO();
            var retorno = new PaginacaoResultadoDto<AreaPromotoraPaginadaDTO>([], 0, 0);
            _mockObterPaginada.Setup(m => m.Executar(filtro)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterAreaPromotoraPaginada(_mockObterPaginada.Object, filtro) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
        }

        [Fact]
        public async Task DadoIdValido_QuandoObterAreaPromotoraPorId_EntaoRetornaDto()
        {
            // Arrange
            var retorno = new AreaPromotoraCompletoDTO();
            _mockObterPorId.Setup(m => m.Executar(1)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterAreaPromotoraPorId(_mockObterPorId.Object, 1) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
        }

        [Fact]
        public async Task DadoRequestValido_QuandoObterAreaPromotoraLista_EntaoRetornaLista()
        {
            // Arrange
            var retorno = new List<RetornoListagemDTO>();
            _mockObterLista.Setup(m => m.Executar()).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterAreaPromotoraLista(_mockObterLista.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
        }

        [Fact]
        public async Task DadoRequestValido_QuandoObterAreaPromotoraListaRedeParceria_EntaoRetornaLista()
        {
            // Arrange
            var retorno = new List<RetornoListagemDTO>();
            _mockObterListaRede.Setup(m => m.Executar()).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterAreaPromotoraListaRedeParceria(_mockObterListaRede.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
        }

        [Fact]
        public async Task DadoDtoValido_QuandoInserirAreaPromotora_EntaoRetornaId()
        {
            // Arrange
            var dto = new AreaPromotoraDTO();
            _mockInserir.Setup(m => m.Executar(dto)).ReturnsAsync(1L);

            // Act
            var resultado = await _sut.InserirAreaPromotora(_mockInserir.Object, dto) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().Be(1L);
        }

        [Fact]
        public async Task DadoDtoEIdValidos_QuandoAlterarAreaPromotora_EntaoRetornaTrue()
        {
            // Arrange
            var dto = new AreaPromotoraDTO();
            _mockAlterar.Setup(m => m.Executar(1, dto)).ReturnsAsync(true);

            // Act
            var resultado = await _sut.AlterarAreaPromotora(_mockAlterar.Object, 1, dto) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().Be(true);
        }

        [Fact]
        public async Task DadoIdValido_QuandoRemoverAreaPromotora_EntaoRetornaTrue()
        {
            // Arrange
            _mockRemover.Setup(m => m.Executar(1)).ReturnsAsync(true);

            // Act
            var resultado = await _sut.RemoverAreaPromotora(_mockRemover.Object, 1) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().Be(true);
        }
    }
}
