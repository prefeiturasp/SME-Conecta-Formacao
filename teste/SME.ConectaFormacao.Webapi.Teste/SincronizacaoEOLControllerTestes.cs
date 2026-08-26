using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Interfaces.SincronizacaoEOL;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using SME.ConectaFormacao.Webapi.Controllers;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SME.ConectaFormacao.Webapi.Teste.Controllers
{
    [ExcludeFromCodeCoverage]
    public class SincronizacaoEOLControllerTestes
    {
        private readonly Mock<ISincronizarFuncaoAtividadeEolUseCase> _mockSincFuncao;
        private readonly Mock<ISincronizarFuncaoAtividadeEolPorDreUseCase> _mockSincFuncaoPorDre;
        private readonly Mock<IExecutarSincronizacaoCargosEolUseCase> _mockSincCargos;
        private readonly SincronizacaoEOLController _sut;

        public SincronizacaoEOLControllerTestes()
        {
            var mocker = new AutoMocker();
            _mockSincFuncao = mocker.GetMock<ISincronizarFuncaoAtividadeEolUseCase>();
            _mockSincFuncaoPorDre = mocker.GetMock<ISincronizarFuncaoAtividadeEolPorDreUseCase>();
            _mockSincCargos = mocker.GetMock<IExecutarSincronizacaoCargosEolUseCase>();
            _sut = mocker.CreateInstance<SincronizacaoEOLController>();
        }

        [Fact]
        public async Task DadoRequestValido_QuandoSincronizarFuncaoAtividade_E_RetornarTrue_EntaoRetornaOk()
        {
            // Arrange
            _mockSincFuncao.Setup(m => m.Executar(It.IsAny<MensagemRabbit>())).ReturnsAsync(true);

            // Act
            var resultado = await _sut.SincronizarFuncaoAtividade() as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().Be("Sincronização de Função Atividade iniciada com sucesso!");
        }

        [Fact]
        public async Task DadoRequestValido_QuandoSincronizarFuncaoAtividade_E_RetornarFalse_EntaoRetornaBadRequest()
        {
            // Arrange
            _mockSincFuncao.Setup(m => m.Executar(It.IsAny<MensagemRabbit>())).ReturnsAsync(false);

            // Act
            var resultado = await _sut.SincronizarFuncaoAtividade() as BadRequestObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            resultado.Value.Should().Be("Erro ao executar sincronização");
        }

        [Fact]
        public async Task DadoCodigoDreValido_QuandoSincronizarFuncaoAtividadePorDre_E_RetornarTrue_EntaoRetornaOk()
        {
            // Arrange
            _mockSincFuncaoPorDre.Setup(m => m.Executar(It.IsAny<MensagemRabbit>())).ReturnsAsync(true);

            // Act
            var resultado = await _sut.SincronizarFuncaoAtividadePorDre("DRE-BT") as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().Be("Sincronização de Função Atividade para DRE DRE-BT executada com sucesso!");
        }
        
        [Fact]
        public async Task DadoCodigoDreInvalido_QuandoSincronizarFuncaoAtividadePorDre_EntaoRetornaBadRequest()
        {
            // Act
            var resultado = await _sut.SincronizarFuncaoAtividadePorDre(" ") as BadRequestObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            resultado.Value.Should().Be("Código da DRE é obrigatório");
        }

        [Fact]
        public async Task DadoRequestValido_QuandoSincronizarCargos_E_RetornarTrue_EntaoRetornaOk()
        {
            // Arrange
            _mockSincCargos.Setup(m => m.Executar(It.IsAny<MensagemRabbit>())).ReturnsAsync(true);

            // Act
            var resultado = await _sut.SincronizarCargos() as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().Be("Sincronização de Cargos EOL executada com sucesso!");
        }
    }
}
