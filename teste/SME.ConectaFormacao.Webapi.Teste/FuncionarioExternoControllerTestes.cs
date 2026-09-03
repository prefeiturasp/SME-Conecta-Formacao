using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos.FuncionarioExterno;
using SME.ConectaFormacao.Aplicacao.Interfaces.FuncionarioExterno.ObterFuncionarioExternoPorCpf;
using SME.ConectaFormacao.Webapi.Controllers;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SME.ConectaFormacao.Webapi.Teste.Controllers
{
    [ExcludeFromCodeCoverage]
    public class FuncionarioExternoControllerTestes
    {
        private readonly Mock<ICasoDeUsoObterFuncionarioExternoPorCpf> _mockUseCase;
        private readonly FuncionarioExternoController _sut;

        public FuncionarioExternoControllerTestes()
        {
            var mocker = new AutoMocker();
            _mockUseCase = mocker.GetMock<ICasoDeUsoObterFuncionarioExternoPorCpf>();
            _sut = mocker.CreateInstance<FuncionarioExternoController>();
        }

        [Fact]
        public async Task DadoCpfValido_QuandoObterFuncionarioExternoPorCfp_EntaoRetornaDto()
        {
            // Arrange
            var cpf = "12345678901";
            var retorno = new FuncionarioExternoDTO("nome", "email", "telefone", "celular", new List<SME.ConectaFormacao.Aplicacao.Dtos.RetornoListagemDTO>());
            _mockUseCase.Setup(m => m.Executar(cpf)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterFuncionarioExternoPorCfp(cpf, _mockUseCase.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockUseCase.Verify(m => m.Executar(cpf), Times.Once);
        }
    }
}
