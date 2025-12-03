using Bogus;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.CargoFuncao;
using SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Interfaces.CargoFuncao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Formacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.PalavraChave;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Webapi.Controllers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ConectaFormacao.Webapi.Teste
{
    public class PublicoControllerTests
    {
        private readonly PublicoController _controller;
        private readonly Faker _faker;

        public PublicoControllerTests()
        {
            _controller = new PublicoController();
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task DadpTipoValido_QuandoObterListaCargoFuncao_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterCargoFuncao>();
            var tipo = CargoFuncaoTipo.Cargo;
            var exibirOpcaoOutros = true;
            var lista = new List<CargoFuncaoDTO>
            {
                new() { Id = 1, Nome = _faker.Name.JobTitle() }
            };

            mockUseCase.Setup(x => x.Executar(tipo, exibirOpcaoOutros)).ReturnsAsync(lista);

            // Act
            var resultado = await _controller.ObterListaCargoFuncao(mockUseCase.Object, tipo, exibirOpcaoOutros);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var retorno = Assert.IsAssignableFrom<IEnumerable<CargoFuncaoDTO>>(okResult.Value);
            Assert.NotEmpty(retorno);
            mockUseCase.Verify(x => x.Executar(tipo, exibirOpcaoOutros), Times.Once);
        }

        [Fact]
        public async Task DadpSolicitacaoValida_QuandoObterListaAreaPromotora_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterAreaPromotoraListaAreaPublica>();
            var lista = new List<RetornoListagemDTO>
            {
                new RetornoListagemDTO { Id = _faker.Random.Long(), Descricao = _faker.Company.CompanyName() }
            };

            mockUseCase.Setup(x => x.Executar()).ReturnsAsync(lista);

            // Act
            var resultado = await _controller.ObterListaAreaPromotora(mockUseCase.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(lista, okResult.Value);
            mockUseCase.Verify(x => x.Executar(), Times.Once);
        }

        [Fact]
        public async Task DadpSolicitacaoValida_QuandoObterListaPalavraChave_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterPalavraChave>();
            var lista = new List<RetornoListagemDTO>
            {
                new RetornoListagemDTO { Id = _faker.Random.Long(), Descricao = _faker.Lorem.Word() }
            };

            mockUseCase.Setup(x => x.Executar()).ReturnsAsync(lista);

            // Act
            var resultado = await _controller.ObterListaPalavraChave(mockUseCase.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(lista, okResult.Value);
            mockUseCase.Verify(x => x.Executar(), Times.Once);
        }

        [Fact]
        public async Task DadpSolicitacaoValida_QuandoObterListaFormato_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterTodosFormatos>();
            var lista = new List<RetornoListagemDTO>
            {
                new RetornoListagemDTO { Id = 1, Descricao = "Presencial" }
            };

            mockUseCase.Setup(x => x.Executar()).ReturnsAsync(lista);

            // Act
            var resultado = await _controller.ObterListaFormato(mockUseCase.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(lista, okResult.Value);
            mockUseCase.Verify(x => x.Executar(), Times.Once);
        }

        [Fact]
        public async Task DadpFiltrosValidos_QuandoObterListagemFormacao_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterListagemFormacaoPaginada>();
            var filtro = new FiltroListagemFormacaoDTO { Titulo = "Curso Teste" };

            var itens = new List<RetornoListagemFormacaoDTO>
            {
                new() { Id = 1, Titulo = "Curso A" }
            };

            // Simulando o objeto de paginação genérico
            var paginacao = new PaginacaoResultadoDTO<RetornoListagemFormacaoDTO>(itens, 1, 1);

            mockUseCase.Setup(x => x.Executar(filtro)).ReturnsAsync(paginacao);

            // Act
            var resultado = await _controller.ObterListagemFormacao(filtro, mockUseCase.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(paginacao, okResult.Value);
            mockUseCase.Verify(x => x.Executar(filtro), Times.Once);
        }

        [Fact]
        public async Task DadpPropostaIdValido_QuandoObterFormacaoDetalhada_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterFormacaoDetalhada>();
            var propostaId = _faker.Random.Long();
            var dtoDetalhe = new RetornoFormacaoDetalhadaDTO
            {
                Titulo = _faker.Commerce.ProductName(),
                AreaPromotora = _faker.Company.CompanyName()
            };

            mockUseCase.Setup(x => x.Executar(propostaId)).ReturnsAsync(dtoDetalhe);

            // Act
            var resultado = await _controller.ObterFormacaoDetalhada(propostaId, mockUseCase.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var retorno = Assert.IsType<RetornoFormacaoDetalhadaDTO>(okResult.Value);
            Assert.Equal(dtoDetalhe.Titulo, retorno.Titulo);
            mockUseCase.Verify(x => x.Executar(propostaId), Times.Once);
        }
    }
}