using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoListarInscritosTurmaCodafCursoNaoHomologadoTestes
    {
        private readonly Mock<IRepositorioCodafCursoNaoHomologadoInscricao> repositorioCodafCursoNaoHomologadoInscricaoMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly CasoDeUsoListarInscritosTurmaCodafCursoNaoHomologado casoDeUsoListarInscritosTurmaCodafCursoNaoHomologado;
        private readonly Faker _faker;

        public CasoDeUsoListarInscritosTurmaCodafCursoNaoHomologadoTestes()
        {
            var mocker = new AutoMocker();

            repositorioCodafCursoNaoHomologadoInscricaoMock = mocker.GetMock<IRepositorioCodafCursoNaoHomologadoInscricao>();
            mapperMock = mocker.GetMock<IMapper>();
            casoDeUsoListarInscritosTurmaCodafCursoNaoHomologado = mocker.CreateInstance<CasoDeUsoListarInscritosTurmaCodafCursoNaoHomologado>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoUmaPropostaTurmaId_QuandoExecutar_DeveRetornarResultadoEsperado()
        {
            // Arrange
            var propostaTurmaId = _faker.Random.Long(1, 1000);
            repositorioCodafCursoNaoHomologadoInscricaoMock
                .Setup(r => r.ObterInscritosPorTurmaAsync(propostaTurmaId, It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new ResultadoPaginado<ResultadoInscritoTurmaCodafCursoNaoHomologadoDto>
                {
                    Itens = [],
                    PaginaAtual = 1,
                    TamanhoPagina = 10,
                    TotalRegistros = 0
                });

            var inscritosDto = new PaginacaoResultadoDto<CodafCursoNaoHomologadoInscritoTurmaDto>([], 0, 1);
            mapperMock.Setup(m => m.Map<List<CodafCursoNaoHomologadoInscritoTurmaDto>>(
                It.IsAny<IEnumerable<ResultadoInscritoTurmaCodafCursoNaoHomologadoDto>>()))
                .Returns([]);

            // Act
            var resultado = await casoDeUsoListarInscritosTurmaCodafCursoNaoHomologado.ExecutarAsync(propostaTurmaId, 1, 10);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.Should().BeEquivalentTo(inscritosDto);
        }
    }
}
