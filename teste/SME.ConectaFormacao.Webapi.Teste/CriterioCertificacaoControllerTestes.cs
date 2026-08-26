using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.CriterioCertificacao;
using SME.ConectaFormacao.Webapi.Controllers;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SME.ConectaFormacao.Webapi.Teste.Controllers
{
    [ExcludeFromCodeCoverage]
    public class CriterioCertificacaoControllerTestes
    {
        private readonly Mock<ICasoDeUsoCriterioCertificacao> _mockUseCase;
        private readonly CriterioCertificacaoController _sut;

        public CriterioCertificacaoControllerTestes()
        {
            var mocker = new AutoMocker();
            _mockUseCase = mocker.GetMock<ICasoDeUsoCriterioCertificacao>();
            _sut = mocker.CreateInstance<CriterioCertificacaoController>();
        }

        [Fact]
        public async Task DadoRequestValido_QuandoObterCriterioCertificacao_EntaoRetornaLista()
        {
            // Arrange
            var retorno = new List<RetornoListagemDTO>();
            _mockUseCase.Setup(m => m.Executar()).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterCriterioCertificacao(_mockUseCase.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockUseCase.Verify(m => m.Executar(), Times.Once);
        }
    }
}
