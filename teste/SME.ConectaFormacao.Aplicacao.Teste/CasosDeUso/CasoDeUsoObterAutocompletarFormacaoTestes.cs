using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.Propostas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterAutocompletarFormacaoTestes
    {
        private readonly Mock<IRepositorioProposta> _repositorioPropostaMock;
        private readonly CasoDeUsoObterAutocompletarFormacao _casoDeUso;
        private readonly Faker _faker;

        public CasoDeUsoObterAutocompletarFormacaoTestes()
        {
            var mocker = new AutoMocker();
            _repositorioPropostaMock = mocker.GetMock<IRepositorioProposta>();
            _casoDeUso = mocker.CreateInstance<CasoDeUsoObterAutocompletarFormacao>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoUmTermoBuscaVazio_QuandoChamarExecutarAsync_EntaoDeveDadosVazio()
        {
            // Arrange
            var filtro = new FiltroAutocompletarNumeroHomologacaoDto
            {
                TermoBusca = "",
                NumeroPagina = 1,
                NumeroRegistros = 10
            };

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(filtro);
            
            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados!.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task DadoUmTermoBuscaValido_QuandoChamarExecutarAsync_EntaoDeveRetornarDados()
        {
            // Arrange
            var filtro = new FiltroAutocompletarNumeroHomologacaoDto
            {
                TermoBusca = _faker.Random.Word(),
                NumeroPagina = 1,
                NumeroRegistros = 10
            };
            var itensMock = new List<AutocompletarNumeroHomologacaoDto>
            {
                new() { NumeroHomologacao = 12345 },
                new() { NumeroHomologacao = 67890 }
            };
            _repositorioPropostaMock
                .Setup(r => r.ObterAutocompletarNumeroHomologacaoAsync(filtro.TermoBusca, filtro.NumeroPagina, filtro.NumeroRegistros))
                .ReturnsAsync(new ResultadoPaginado<AutocompletarNumeroHomologacaoDto>() { Itens = itensMock, PaginaAtual = 1, TamanhoPagina = 10, TotalRegistros = itensMock.Count });
            // Act
            var resultado = await _casoDeUso.ExecutarAsync(filtro);
            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados!.Items.Should().HaveCount(2);
            resultado.Dados.Items.Should().BeEquivalentTo(itensMock);
        }
    }
}
