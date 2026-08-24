using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafDeclaracoes;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafDeclaracoes;
using SME.ConectaFormacao.Webapi.Controllers;

namespace SME.ConectaFormacao.Webapi.Teste
{
    public class CodafDeclaracaoControllerTestes
    {
        private readonly Mock<ICasoDeUsoEmitirDeclaracaoCodaf> _casoDeUsoEmitirDeclaracaoCodafMock;
        private readonly Mock<ICasoDeUsoListarMinhasDeclaracoesCodaf> _casoDeUsoListarMinhasDeclaracoesCodafMock;
        private readonly CodafDeclaracaoController _sut;
        private readonly Faker _faker;

        public CodafDeclaracaoControllerTestes()
        {
            var mocker = new AutoMocker();
            _casoDeUsoEmitirDeclaracaoCodafMock = mocker.GetMock<ICasoDeUsoEmitirDeclaracaoCodaf>();
            _casoDeUsoListarMinhasDeclaracoesCodafMock = mocker.GetMock<ICasoDeUsoListarMinhasDeclaracoesCodaf>();

            _sut = mocker.CreateInstance<CodafDeclaracaoController>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoCodafNaoHomologadoIdValido_QuandoChamarEmitirDeclaracoesCodaf_EntaoDeveRetornarOk()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);
            var resultadoSucesso = Resultado.DeSucesso();

            _casoDeUsoEmitirDeclaracaoCodafMock
                .Setup(c => c.ExecutarAsync(It.IsAny<long>()))
                .ReturnsAsync(resultadoSucesso);

            // Act
            var resultado = await _sut.EmitirDeclaracoesCodaf(id) as NoContentResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado!.StatusCode.Should().Be(204);
            _casoDeUsoEmitirDeclaracaoCodafMock.Verify(c => c.ExecutarAsync(id), Times.Once);
        }

        [Fact]
        public async Task DadoFiltroValido_QuandoChamarListarMinhasDeclaracoes_EntaoDeveRetornarOkComDados()
        {
            // Arrange
            var filtro = new FiltroListaMinhasDeclaracoesCodafDto { NumeroPagina = 1, NumeroRegistros = 10 };
            var paginacaoDto = new PaginacaoResultadoDto<MinhasDeclaracoesCodafDto>([], 0, 10);
            var resultadoSucesso = Resultado<PaginacaoResultadoDto<MinhasDeclaracoesCodafDto>>.DeSucesso(paginacaoDto);

            _casoDeUsoListarMinhasDeclaracoesCodafMock
                .Setup(c => c.ExecutarAsync(filtro))
                .ReturnsAsync(resultadoSucesso);

            // Act
            var resultado = await _sut.ListarMinhasDeclaracoes(filtro) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado!.StatusCode.Should().Be(200);
            _casoDeUsoListarMinhasDeclaracoesCodafMock.Verify(c => c.ExecutarAsync(filtro), Times.Once);
        }
    }
}
