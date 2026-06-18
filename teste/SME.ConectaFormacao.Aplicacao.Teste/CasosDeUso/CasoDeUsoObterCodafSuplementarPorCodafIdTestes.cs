using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterCodafSuplementarPorCodafIdTestes
    {
        private readonly Mock<IRepositorioCodafListaPresenca> _repositorioCodafListaPresencaMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CasoDeUsoObterCodafSuplementarPorCodafId _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterCodafSuplementarPorCodafIdTestes()
        {
            var mocker = new AutoMocker();
            _repositorioCodafListaPresencaMock = mocker.GetMock<IRepositorioCodafListaPresenca>();
            _mapperMock = mocker.GetMock<IMapper>();
            _sut = mocker.CreateInstance<CasoDeUsoObterCodafSuplementarPorCodafId>();
            _faker = new();
        }

        [Fact]
        public async Task DadoCodafIdValido_QuandoChamarExecutar_DeveRetornarResultadoEsperado()
        {
            // Arrange
            var codafId = _faker.Random.Long(1);
            var listaPresencaEntidade = new CodafListaPresenca(
                propostaId: 1,
                propostaTurmaId: 1,
                new(DataPublicacao: DateTime.Now,
                DataPublicacaoDom: DateTime.Now,
                NumeroComunicado: 123,
                PaginaComunicadoDom: 12,
                CodigoCursoEol: 1,
                CodigoNivel: 2,
                Observacao: "Observação teste"),
                Perfis.ADMIN_DF);

            var codaSuplementarDto = new CodafSuplementarDetalhadoDto
            {
                Id = codafId,
                PropostaId = 1,
                PropostaTurmaId = 1,
                DataPublicacao = DateTime.Now,
                DataPublicacaoDom = DateTime.Now,
                NumeroComunicado = 123,
                PaginaComunicadoDom = 12,
                CodigoCursoEol = 1,
                CodigoNivel = 2,
                Observacao = "Observação teste",
            };

            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(codafId))
                .ReturnsAsync(listaPresencaEntidade);
            _mapperMock
                .Setup(m => m.Map<CodafSuplementarDetalhadoDto>(listaPresencaEntidade))
                .Returns(codaSuplementarDto);

            // Act
            var resultado = await _sut.ExecutarAsync(codafId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.Id.Should().Be(codafId);
        }

        [Fact]
        public async Task DadoCodafIdInvalido_QuandoChamarExecutar_DeveRetornarErroNaoEncontrado()
        {
            // Arrange
            var codafId = _faker.Random.Long(1);
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(codafId))
                .ReturnsAsync((CodafListaPresenca?)null);
            // Act
            var resultado = await _sut.ExecutarAsync(codafId);
            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().NotBeNull();
            resultado.MensagensErro.Should().Contain("Codaf não encontrado.");
        }
    }
}
