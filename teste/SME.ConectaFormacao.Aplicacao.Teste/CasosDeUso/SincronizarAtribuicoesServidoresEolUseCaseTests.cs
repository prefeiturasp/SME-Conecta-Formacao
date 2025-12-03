using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.SincronizacaoEOL;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class SincronizarAtribuicoesServidoresEolUseCaseTests
    {
        private readonly Mock<IServicoEol> _servicoEolMock;
        private readonly Mock<IRepositorioSincronizador> _repositorioSincronizadorMock;
        private readonly Mock<IRepositorioAtribuicaoAulaServidor> _repositorioAtribuicaoAulaServidorMock;
        private readonly SincronizarAtribuicoesServidoresEolUseCase _useCase;

        public SincronizarAtribuicoesServidoresEolUseCaseTests()
        {
            var mocker = new AutoMocker();

            _servicoEolMock = mocker.GetMock<IServicoEol>();
            _repositorioSincronizadorMock = mocker.GetMock<IRepositorioSincronizador>();
            _repositorioAtribuicaoAulaServidorMock = mocker.GetMock<IRepositorioAtribuicaoAulaServidor>();
            _useCase = mocker.CreateInstance<SincronizarAtribuicoesServidoresEolUseCase>();
        }

        [Fact]
        public async Task DadoQueNaoExistemAtribuicoesParaSincronizar_QuandoExecutar_EntaoDeveRetornarVerdadeiroSemProcessarDados()
        {
            // Arrange
            var dataUltimaAtualizacao = DateTime.Now.AddDays(-1);
            _repositorioAtribuicaoAulaServidorMock
                .Setup(r => r.ObterDataUltimaAtualizacaoAsync())
                .ReturnsAsync(dataUltimaAtualizacao);

            // Act
            var resultado = await _useCase.Executar(new());

            // Assert
            resultado.Should().BeTrue();
            _repositorioAtribuicaoAulaServidorMock.Verify(r => r.ObterDataUltimaAtualizacaoAsync(), Times.Once);
            _servicoEolMock.Verify(s => s.ObterAtribuicoesServidorEolPorDataAtualizacaoAsync(It.IsAny<DateTime?>()), Times.Once);
            _repositorioSincronizadorMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task DadoQueExistemAtribuicoesParaSincronizar_QuandoExecutar_EntaoDeveProcessarDadosCorretamente()
        {
            // Arrange
            var dataUltimaAtualizacao = DateTime.Now.AddDays(-1);
            var atribuicoesEol = new List<AtribuicaoServidorEolDto>
            {
                new()
                {
                    CdEtapaEnsino = 1,
                    AnoSerie = "2022",
                    CdComponenteCurricular = 258,
                    CdRegistroFuncional = "12345",
                    CodigoUe = "UE001",
                    Excluido = false
                }
            };
            _repositorioAtribuicaoAulaServidorMock
                .Setup(r => r.ObterDataUltimaAtualizacaoAsync())
                .ReturnsAsync(dataUltimaAtualizacao);
            _servicoEolMock
                .Setup(s => s.ObterAtribuicoesServidorEolPorDataAtualizacaoAsync(dataUltimaAtualizacao))
                .ReturnsAsync(atribuicoesEol);
            // Act
            var resultado = await _useCase.Executar(new());
            // Assert
            resultado.Should().BeTrue();
            _repositorioAtribuicaoAulaServidorMock.Verify(r => r.ObterDataUltimaAtualizacaoAsync(), Times.Once);
            _servicoEolMock.Verify(s => s.ObterAtribuicoesServidorEolPorDataAtualizacaoAsync(dataUltimaAtualizacao), Times.Once);
            _repositorioSincronizadorMock.Verify(r => r.LimparAtribuicaoServidorEolAsync(It.IsAny<List<string>>()), Times.Once);
            _repositorioSincronizadorMock.Verify(r => r.SincronizarLoteAtribuicaoServidorEolAsync(It.IsAny<List<Dominio.Entidades.AtribuicaoServidorEol>>()), Times.Once);
        }
    }
}
