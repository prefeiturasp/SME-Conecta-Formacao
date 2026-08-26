using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.ComponenteCurricular;
using SME.ConectaFormacao.Aplicacao.Interfaces.ComponenteCurricular;
using SME.ConectaFormacao.Webapi.Controllers;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SME.ConectaFormacao.Webapi.Teste.Controllers
{
    [ExcludeFromCodeCoverage]
    public class ComponenteCurricularControllerTestes
    {
        private readonly Mock<ICasoDeUsoObterListaComponentesCurriculares> _mockObterLista;
        private readonly ComponenteCurricularController _sut;

        public ComponenteCurricularControllerTestes()
        {
            var mocker = new AutoMocker();
            _mockObterLista = mocker.GetMock<ICasoDeUsoObterListaComponentesCurriculares>();
            _sut = mocker.CreateInstance<ComponenteCurricularController>();
        }

        [Fact]
        public async Task DadoFiltroValido_QuandoObterListaComponentesCurriculares_EntaoRetornaLista()
        {
            // Arrange
            var filtro = new FiltroListaComponenteCurricularDTO();
            var retorno = new List<RetornoListagemTodosDTO>();
            _mockObterLista.Setup(m => m.Executar(filtro)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterListaComponentesCurriculares(_mockObterLista.Object, filtro) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockObterLista.Verify(m => m.Executar(filtro), Times.Once);
        }
    }
}
