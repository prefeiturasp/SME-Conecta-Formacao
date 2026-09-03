using Bogus;
using ConectaFormacao.Dominio.Servicos;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafDeclaracoes;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafDeclaracoes;
using SME.ConectaFormacao.Infra.Dados.Estrategias.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoEmitirDeclaracaoCodafTestes
    {
        private readonly CasoDeUsoEmitirDeclaracaoCodaf _sut;
        private readonly Faker _faker;
        private readonly AutoMocker _mocker;
        private readonly Mock<IDbTransaction> _dbTransactionMock;

        public CasoDeUsoEmitirDeclaracaoCodafTestes()
        {
            _mocker = new AutoMocker();
            _faker = new Faker("pt_BR");

            _dbTransactionMock = new Mock<IDbTransaction>();
            _mocker.GetMock<ITransacao>()
                   .Setup(t => t.Iniciar())
                   .Returns(_dbTransactionMock.Object);

            _sut = _mocker.CreateInstance<CasoDeUsoEmitirDeclaracaoCodaf>();
        }

        [Fact]
        public async Task DadoNenhumDadoParaEmissaoQuandoExecutarAsyncEntaoDeveRetornarErroNaoEncontrado()
        {
            // Arrange
            var codafNaoHomologadoId = _faker.Random.Long(1, 100);

            _mocker.GetMock<IRepositorioCodafDeclaracao>()
                .Setup(x => x.ObterDadosParaEmissaoDeclaracoesCodafAsync(codafNaoHomologadoId))
                .ReturnsAsync([]);

            // Act
            var resultado = await _sut.ExecutarAsync(codafNaoHomologadoId);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);

            _mocker.GetMock<ITransacao>().Verify(t => t.Iniciar(), Times.Never);
        }

        [Fact]
        public async Task DadoRepositoriNuloQuandoExecutarAsyncEntaoDeveLancarInvalidOperationException()
        {
            // Arrange
            var codafNaoHomologadoId = _faker.Random.Long(1, 100);

            _mocker.GetMock<IRepositorioCodafDeclaracao>()
                .Setup(x => x.ObterDadosParaEmissaoDeclaracoesCodafAsync(codafNaoHomologadoId))
                .ReturnsAsync((List<DadosEmissaoDeclaracaoCodafDto>)null!);

            // Act
            var acao = async () => await _sut.ExecutarAsync(codafNaoHomologadoId);

            // Assert
            await acao.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Os dados para emissão de declarações do Codaf não foram encontrados.");
        }

        [Fact]
        public async Task DadoCodafComCursistaComRfQuandoExecutarAsyncEntaoDeveEmitirDeclaracaoComSanitizacao()
        {
            // Arrange
            var codafNaoHomologadoId = _faker.Random.Long(1, 100);
            var dadosEmissao = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Cursista, temRf: true);
            var periodo = CriarPeriodoFake();
            var inscritosReprovados = new List<long> { _faker.Random.Long(1000, 2000) };

            SetupDependenciasDeGeracao(dadosEmissao, periodo, TipoEstrategiaCodaf.CursistaComRf);

            _mocker.GetMock<IRepositorioCodafDeclaracao>()
                .Setup(x => x.ObterDadosParaEmissaoDeclaracoesCodafAsync(codafNaoHomologadoId))
                .ReturnsAsync([dadosEmissao]);

            _mocker.GetMock<IRepositorioCodafSuplementarInscricao>()
                .Setup(x => x.ObterIdInscritosReprovadosAsync(codafNaoHomologadoId))
                .ReturnsAsync(inscritosReprovados);

            // Act
            var resultado = await _sut.ExecutarAsync(codafNaoHomologadoId);

            // Assert
            resultado.Sucesso.Should().BeTrue();

            var idsEsperadosParaSanitizacao = new List<long> { dadosEmissao.InscricaoId }.Union(inscritosReprovados).ToList();

            _mocker.GetMock<IRepositorioCodafDeclaracao>().Verify(x => x.InativarDeclaracoesAnterioresCursistaAsync(It.Is<IEnumerable<long>>(ids => ids.SequenceEqual(idsEsperadosParaSanitizacao))), Times.Once);
            _mocker.GetMock<IRepositorioCodafDeclaracao>().Verify(x => x.InserirLoteAsync(It.Is<IEnumerable<CodafDeclaracao>>(l => l.Count() == 1)), Times.Once);
            _mocker.GetMock<IRepositorioCodafDeclaracao>().Verify(x => x.AtualizaCodigoDeclaracao(codafNaoHomologadoId), Times.Once);
            _dbTransactionMock.Verify(t => t.Commit(), Times.Once);
            _mocker.GetMock<IMediator>().Verify(x => x.Send(It.Is<PublicarNaFilaRabbitCommand>(cmd => cmd.Rota == RotasRabbit.GerarArquivoDeclaracoesCodaf), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoCodafComRegenteComRfQuandoExecutarAsyncEntaoDeveEmitirDeclaracaoComSanitizacaoVazia()
        {
            // Arrange
            var codafNaoHomologadoId = _faker.Random.Long(1, 100);
            var dadosEmissao = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Regente, temRf: true);
            var periodo = CriarPeriodoFake();
            var inscritosReprovados = new List<long>();

            SetupDependenciasDeGeracao(dadosEmissao, periodo, TipoEstrategiaCodaf.RegenteComRf);

            _mocker.GetMock<IRepositorioCodafDeclaracao>()
                .Setup(x => x.ObterDadosParaEmissaoDeclaracoesCodafAsync(codafNaoHomologadoId))
                .ReturnsAsync([dadosEmissao]);

            _mocker.GetMock<IRepositorioCodafSuplementarInscricao>()
                .Setup(x => x.ObterIdInscritosReprovadosAsync(codafNaoHomologadoId))
                .ReturnsAsync(inscritosReprovados);

            // Act
            var resultado = await _sut.ExecutarAsync(codafNaoHomologadoId);

            // Assert
            resultado.Sucesso.Should().BeTrue();

            // Sanitização é chamada mesmo para regentes, com lista vazia
            _mocker.GetMock<IRepositorioCodafDeclaracao>().Verify(x => x.InativarDeclaracoesAnterioresCursistaAsync(It.Is<IEnumerable<long>>(l => l.Count() == 0)), Times.Once);
            _mocker.GetMock<IRepositorioCodafDeclaracao>().Verify(x => x.InserirLoteAsync(It.Is<IEnumerable<CodafDeclaracao>>(l => l.Count() == 1)), Times.Once);
            _mocker.GetMock<IRepositorioCodafDeclaracao>().Verify(x => x.AtualizaCodigoDeclaracao(codafNaoHomologadoId), Times.Once);
            _dbTransactionMock.Verify(t => t.Commit(), Times.Once);
        }

        [Fact]
        public async Task DadoCodafComCurstaSemRfQuandoExecutarAsyncEntaoDeveUtilizarEstrategiaCursistaSemRf()
        {
            // Arrange
            var codafNaoHomologadoId = _faker.Random.Long(1, 100);
            var dadosEmissao = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Cursista, temRf: false);
            var periodo = CriarPeriodoFake();
            var inscritosReprovados = new List<long>();

            SetupDependenciasDeGeracao(dadosEmissao, periodo, TipoEstrategiaCodaf.CursistaSemRf);

            _mocker.GetMock<IRepositorioCodafDeclaracao>()
                .Setup(x => x.ObterDadosParaEmissaoDeclaracoesCodafAsync(codafNaoHomologadoId))
                .ReturnsAsync([dadosEmissao]);

            _mocker.GetMock<IRepositorioCodafSuplementarInscricao>()
                .Setup(x => x.ObterIdInscritosReprovadosAsync(codafNaoHomologadoId))
                .ReturnsAsync(inscritosReprovados);

            // Act
            var resultado = await _sut.ExecutarAsync(codafNaoHomologadoId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _mocker.GetMock<IKeyedServiceProvider>().Verify(x => x.GetRequiredKeyedService(typeof(IDeclaracaoCodafGeradorConteudo), (object)TipoEstrategiaCodaf.CursistaSemRf), Times.Once);
            _dbTransactionMock.Verify(t => t.Commit(), Times.Once);
        }

        [Fact]
        public async Task DadoCodafComRegenteSeRfQuandoExecutarAsyncEntaoDeveUtilizarEstrategiaRegenteSemRf()
        {
            // Arrange
            var codafNaoHomologadoId = _faker.Random.Long(1, 100);
            var dadosEmissao = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Regente, temRf: false);
            var periodo = CriarPeriodoFake();
            var inscritosReprovados = new List<long>();

            SetupDependenciasDeGeracao(dadosEmissao, periodo, TipoEstrategiaCodaf.RegenteSemRf);

            _mocker.GetMock<IRepositorioCodafDeclaracao>()
                .Setup(x => x.ObterDadosParaEmissaoDeclaracoesCodafAsync(codafNaoHomologadoId))
                .ReturnsAsync([dadosEmissao]);

            _mocker.GetMock<IRepositorioCodafSuplementarInscricao>()
                .Setup(x => x.ObterIdInscritosReprovadosAsync(codafNaoHomologadoId))
                .ReturnsAsync(inscritosReprovados);

            // Act
            var resultado = await _sut.ExecutarAsync(codafNaoHomologadoId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _mocker.GetMock<IKeyedServiceProvider>().Verify(x => x.GetRequiredKeyedService(typeof(IDeclaracaoCodafGeradorConteudo), (object)TipoEstrategiaCodaf.RegenteSemRf), Times.Once);
            _dbTransactionMock.Verify(t => t.Commit(), Times.Once);
        }

        [Fact]
        public async Task DadoExcecaoNaInsercaoQuandoExecutarAsyncEntaoDeveFazerRollbackELancarExcecao()
        {
            // Arrange
            var codafNaoHomologadoId = _faker.Random.Long(1, 100);
            var dadosEmissao = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Cursista, temRf: true);
            var periodo = CriarPeriodoFake();
            var inscritosReprovados = new List<long>();

            SetupDependenciasDeGeracao(dadosEmissao, periodo, TipoEstrategiaCodaf.CursistaComRf);

            _mocker.GetMock<IRepositorioCodafDeclaracao>()
                .Setup(x => x.ObterDadosParaEmissaoDeclaracoesCodafAsync(codafNaoHomologadoId))
                .ReturnsAsync([dadosEmissao]);

            _mocker.GetMock<IRepositorioCodafSuplementarInscricao>()
                .Setup(x => x.ObterIdInscritosReprovadosAsync(codafNaoHomologadoId))
                .ReturnsAsync(inscritosReprovados);

            _mocker.GetMock<IRepositorioCodafDeclaracao>()
                .Setup(x => x.InserirLoteAsync(It.IsAny<IEnumerable<CodafDeclaracao>>()))
                .ThrowsAsync(new Exception("Erro de banco de dados simulado"));

            // Act
            var acao = async () => await _sut.ExecutarAsync(codafNaoHomologadoId);

            // Assert
            await acao.Should().ThrowAsync<Exception>().WithMessage("Erro de banco de dados simulado");
            _dbTransactionMock.Verify(t => t.Rollback(), Times.Once);
            _dbTransactionMock.Verify(t => t.Commit(), Times.Never);
        }

        [Fact]
        public async Task DadoPeriodoNuloQuandoExecutarAsyncEntaoDeveProcessarComDatasNulas()
        {
            // Arrange
            var codafNaoHomologadoId = _faker.Random.Long(1, 100);
            var dadosEmissao = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Cursista, temRf: false);
            var inscritosReprovados = new List<long>();

            SetupDependenciasDeGeracao(dadosEmissao, null, TipoEstrategiaCodaf.CursistaSemRf);

            _mocker.GetMock<IRepositorioCodafDeclaracao>()
                .Setup(x => x.ObterDadosParaEmissaoDeclaracoesCodafAsync(codafNaoHomologadoId))
                .ReturnsAsync([dadosEmissao]);

            _mocker.GetMock<IRepositorioCodafSuplementarInscricao>()
                .Setup(x => x.ObterIdInscritosReprovadosAsync(codafNaoHomologadoId))
                .ReturnsAsync(inscritosReprovados);

            // Act
            var resultado = await _sut.ExecutarAsync(codafNaoHomologadoId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _mocker.GetMock<IRepositorioCodafDeclaracao>().Verify(
                x => x.InserirLoteAsync(It.Is<IEnumerable<CodafDeclaracao>>(lista =>
                    !lista.First().MetadadosJson!.Contains("\"dataInicio\":\"") &&
                    !lista.First().MetadadosJson!.Contains("\"dataFim\":\""))),
                Times.Once);
        }

        [Fact]
        public async Task DadoMultiplosParticipantesQuandoExecutarAsyncEntaoDeveUtilizarEstrategiasCorrespondentes()
        {
            // Arrange
            var codafNaoHomologadoId = _faker.Random.Long(1, 100);

            var dadosCursistaSemRf = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Cursista, false);
            var dadosCursistaComRf = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Cursista, true);
            var dadosRegenteComRf = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Regente, true);

            var periodo = CriarPeriodoFake();

            SetupDependenciasDeGeracao(dadosCursistaSemRf, periodo, TipoEstrategiaCodaf.CursistaSemRf);
            SetupDependenciasDeGeracao(dadosCursistaComRf, periodo, TipoEstrategiaCodaf.CursistaComRf);
            SetupDependenciasDeGeracao(dadosRegenteComRf, periodo, TipoEstrategiaCodaf.RegenteComRf);

            _mocker.GetMock<IRepositorioCodafDeclaracao>()
                .Setup(x => x.ObterDadosParaEmissaoDeclaracoesCodafAsync(codafNaoHomologadoId))
                .ReturnsAsync([dadosCursistaSemRf, dadosCursistaComRf, dadosRegenteComRf]);

            // Act
            var resultado = await _sut.ExecutarAsync(codafNaoHomologadoId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _mocker.GetMock<IRepositorioCodafDeclaracao>().Verify(x => x.InserirLoteAsync(It.Is<IEnumerable<CodafDeclaracao>>(lista => lista.Count() == 3)), Times.Once);

            _mocker.GetMock<IKeyedServiceProvider>().Verify(x => x.GetRequiredKeyedService(typeof(IDeclaracaoCodafGeradorConteudo), (object)TipoEstrategiaCodaf.CursistaSemRf), Times.Once);
            _mocker.GetMock<IKeyedServiceProvider>().Verify(x => x.GetRequiredKeyedService(typeof(IDeclaracaoCodafGeradorConteudo), (object)TipoEstrategiaCodaf.CursistaComRf), Times.Once);
            _mocker.GetMock<IKeyedServiceProvider>().Verify(x => x.GetRequiredKeyedService(typeof(IDeclaracaoCodafGeradorConteudo), (object)TipoEstrategiaCodaf.RegenteComRf), Times.Once);
        }

        [Fact]
        public async Task DadoCursistaComInscritosReprovadosQuandoExecutarAsyncEntaoDeveSanitizarAmbos()
        {
            // Arrange
            var codafNaoHomologadoId = _faker.Random.Long(1, 100);
            var dadosEmissao = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Cursista, temRf: false);
            var periodo = CriarPeriodoFake();
            var inscricaoId = _faker.Random.Long(100, 500);
            dadosEmissao.InscricaoId = inscricaoId;

            var inscritosReprovados = new List<long> 
            { 
                _faker.Random.Long(1000, 2000),
                _faker.Random.Long(2000, 3000)
            };

            SetupDependenciasDeGeracao(dadosEmissao, periodo, TipoEstrategiaCodaf.CursistaSemRf);

            _mocker.GetMock<IRepositorioCodafDeclaracao>()
                .Setup(x => x.ObterDadosParaEmissaoDeclaracoesCodafAsync(codafNaoHomologadoId))
                .ReturnsAsync([dadosEmissao]);

            _mocker.GetMock<IRepositorioCodafSuplementarInscricao>()
                .Setup(x => x.ObterIdInscritosReprovadosAsync(codafNaoHomologadoId))
                .ReturnsAsync(inscritosReprovados);

            // Act
            var resultado = await _sut.ExecutarAsync(codafNaoHomologadoId);

            // Assert
            resultado.Sucesso.Should().BeTrue();

            var idsEsperados = new List<long> { inscricaoId }.Union(inscritosReprovados).ToList();
            _mocker.GetMock<IRepositorioCodafDeclaracao>().Verify(
                x => x.InativarDeclaracoesAnterioresCursistaAsync(It.Is<IEnumerable<long>>(ids => ids.Count() == idsEsperados.Count && ids.SequenceEqual(idsEsperados))), 
                Times.Once);
        }

        [Fact]
        public async Task DadoMetadadosQuandoExecutarAsyncEntaoDeveSerializarCorretos()
        {
            // Arrange
            var codafNaoHomologadoId = _faker.Random.Long(1, 100);
            var dadosEmissao = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Regente, temRf: true);
            var periodo = CriarPeriodoFake();

            SetupDependenciasDeGeracao(dadosEmissao, periodo, TipoEstrategiaCodaf.RegenteComRf);

            _mocker.GetMock<IRepositorioCodafDeclaracao>()
                .Setup(x => x.ObterDadosParaEmissaoDeclaracoesCodafAsync(codafNaoHomologadoId))
                .ReturnsAsync([dadosEmissao]);

            // Act
            var resultado = await _sut.ExecutarAsync(codafNaoHomologadoId);

            // Assert
            resultado.Sucesso.Should().BeTrue();

            _mocker.GetMock<IRepositorioCodafDeclaracao>().Verify(
                x => x.InserirLoteAsync(It.Is<IEnumerable<CodafDeclaracao>>(lista =>
                    lista.First().MetadadosJson!.Contains("\"nomeFormacao\"") &&
                    lista.First().MetadadosJson!.Contains("\"horasTotais\"") &&
                    lista.First().MetadadosJson!.Contains("\"cargaHorariaTotalOutra\"") &&
                    lista.First().MetadadosJson!.Contains("\"emissor\"") &&
                    lista.First().MetadadosJson!.Contains("\"tipoFormacao\"") &&
                    lista.First().MetadadosJson!.Contains("\"dataInicio\"") &&
                    lista.First().MetadadosJson!.Contains("\"dataFim\""))),
                Times.Once);
        }

        [Fact]
        public async Task DadoEntidadesVaziaQuandoExecutarAsyncEntaoNaoDeveSalvarNemCommitar()
        {
            // Arrange
            var codafNaoHomologadoId = _faker.Random.Long(1, 100);

            _mocker.GetMock<IRepositorioCodafDeclaracao>()
                .Setup(x => x.ObterDadosParaEmissaoDeclaracoesCodafAsync(codafNaoHomologadoId))
                .ReturnsAsync((List<DadosEmissaoDeclaracaoCodafDto>)null!);

            // Act
            var acao = async () => await _sut.ExecutarAsync(codafNaoHomologadoId);

            // Assert
            await acao.Should().ThrowAsync<InvalidOperationException>();
            _mocker.GetMock<ITransacao>().Verify(t => t.Iniciar(), Times.Never);
            _mocker.GetMock<IRepositorioCodafDeclaracao>().Verify(x => x.InserirLoteAsync(It.IsAny<IEnumerable<CodafDeclaracao>>()), Times.Never);
        }

        [Fact]
        public async Task DadoRegenteQuandoExecutarAsyncEntaoNaoDeveSanitizar()
        {
            // Arrange
            var codafNaoHomologadoId = _faker.Random.Long(1, 100);
            var dadosEmissao = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Regente, temRf: false);
            var periodo = CriarPeriodoFake();
            var inscritosReprovados = new List<long>();

            SetupDependenciasDeGeracao(dadosEmissao, periodo, TipoEstrategiaCodaf.RegenteSemRf);

            _mocker.GetMock<IRepositorioCodafDeclaracao>()
                .Setup(x => x.ObterDadosParaEmissaoDeclaracoesCodafAsync(codafNaoHomologadoId))
                .ReturnsAsync([dadosEmissao]);

            _mocker.GetMock<IRepositorioCodafSuplementarInscricao>()
                .Setup(x => x.ObterIdInscritosReprovadosAsync(codafNaoHomologadoId))
                .ReturnsAsync(inscritosReprovados);

            // Act
            var resultado = await _sut.ExecutarAsync(codafNaoHomologadoId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _mocker.GetMock<IRepositorioCodafDeclaracao>().Verify(
                x => x.InativarDeclaracoesAnterioresCursistaAsync(It.Is<IEnumerable<long>>(ids => ids.Count() == 0)), 
                Times.Once);
        }

        [Fact]
        public async Task DadoEmissaoComSucessoQuandoExecutarAsyncEntaoDevePubicarNaFila()
        {
            // Arrange
            var codafNaoHomologadoId = _faker.Random.Long(1, 100);
            var dadosEmissao = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Cursista, temRf: true);
            var periodo = CriarPeriodoFake();

            SetupDependenciasDeGeracao(dadosEmissao, periodo, TipoEstrategiaCodaf.CursistaComRf);

            _mocker.GetMock<IRepositorioCodafDeclaracao>()
                .Setup(x => x.ObterDadosParaEmissaoDeclaracoesCodafAsync(codafNaoHomologadoId))
                .ReturnsAsync([dadosEmissao]);

            // Act
            var resultado = await _sut.ExecutarAsync(codafNaoHomologadoId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _mocker.GetMock<IMediator>().Verify(
                x => x.Send(It.Is<PublicarNaFilaRabbitCommand>(cmd => cmd.Rota == RotasRabbit.GerarArquivoDeclaracoesCodaf && (long)cmd.Filtros == codafNaoHomologadoId), It.IsAny<CancellationToken>()), 
                Times.Once);
        }

        [Fact]
        public async Task DadoEmissaoComPeriodoQuandoExecutarAsyncEntaoDeveAtualizarDatasPeriodo()
        {
            // Arrange
            var codafNaoHomologadoId = _faker.Random.Long(1, 100);
            var dadosEmissao = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Cursista, temRf: false);
            var periodo = CriarPeriodoFake();

            SetupDependenciasDeGeracao(dadosEmissao, periodo, TipoEstrategiaCodaf.CursistaSemRf);

            _mocker.GetMock<IRepositorioCodafDeclaracao>()
                .Setup(x => x.ObterDadosParaEmissaoDeclaracoesCodafAsync(codafNaoHomologadoId))
                .ReturnsAsync([dadosEmissao]);

            // Act
            var resultado = await _sut.ExecutarAsync(codafNaoHomologadoId);

            // Assert
            resultado.Sucesso.Should().BeTrue();

            _mocker.GetMock<IRepositorioCodafDeclaracao>().Verify(
                x => x.InserirLoteAsync(It.Is<IEnumerable<CodafDeclaracao>>(lista =>
                    lista.First().MetadadosJson!.Contains($"\"dataInicio\":\"{periodo.DataInicio:yyyy-MM-dd}\"") ||
                    lista.First().MetadadosJson!.Contains($"\"dataInicio\""))),
                Times.Once);
        }

        [Fact]
        public async Task DadoApenasRegentesQuandoExecutarAsyncEntaoDeveSanitizarComListaVazia()
        {
            // Arrange
            var codafNaoHomologadoId = _faker.Random.Long(1, 100);
            var dadosRegenteComRf = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Regente, true);
            var dadosRegenteSemRf = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Regente, false);
            var periodo = CriarPeriodoFake();
            var inscritosReprovados = new List<long> { _faker.Random.Long(1000, 2000) };

            SetupDependenciasDeGeracao(dadosRegenteComRf, periodo, TipoEstrategiaCodaf.RegenteComRf);
            SetupDependenciasDeGeracao(dadosRegenteSemRf, periodo, TipoEstrategiaCodaf.RegenteSemRf);

            _mocker.GetMock<IRepositorioCodafDeclaracao>()
                .Setup(x => x.ObterDadosParaEmissaoDeclaracoesCodafAsync(codafNaoHomologadoId))
                .ReturnsAsync([dadosRegenteComRf, dadosRegenteSemRf]);

            _mocker.GetMock<IRepositorioCodafSuplementarInscricao>()
                .Setup(x => x.ObterIdInscritosReprovadosAsync(codafNaoHomologadoId))
                .ReturnsAsync(inscritosReprovados);

            // Act
            var resultado = await _sut.ExecutarAsync(codafNaoHomologadoId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            // Sanitização é chamada mesmo para regentem, mas com apenas os inscritos reprovados
            _mocker.GetMock<IRepositorioCodafDeclaracao>().Verify(
                x => x.InativarDeclaracoesAnterioresCursistaAsync(It.Is<IEnumerable<long>>(ids => ids.SequenceEqual(inscritosReprovados))), 
                Times.Once);
            _mocker.GetMock<IRepositorioCodafDeclaracao>().Verify(x => x.InserirLoteAsync(It.Is<IEnumerable<CodafDeclaracao>>(l => l.Count() == 2)), Times.Once);
        }

        private DadosEmissaoDeclaracaoCodafDto CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf tipoParticipacao, bool temRf)
        {
            return new DadosEmissaoDeclaracaoCodafDto
            {
                IdReferencia = _faker.Random.Long(1, 100),
                InscricaoId = _faker.Random.Long(101, 200),
                PropostaTurmaId = _faker.Random.Long(201, 300),
                NomeCompleto = _faker.Person.FullName,
                Documento = _faker.Random.String2(11, "0123456789"),
                TemRf = temRf,
                TipoParticipacao = tipoParticipacao,
                NomeFormacao = "Formacao Fake",
                HorasTotais = _faker.Random.Int(10, 40),
                Emissor = "DRE Local",
                TipoFormacao = "curso",
                CargaHorariaTotalOutra = "10h",
                DataRealizacao = _faker.Date.Recent(),
                DataPublicacao = _faker.Date.Recent(),
                EmailUsuario = _faker.Internet.Email()
            };
        }

        private PeriodoRealizacao CriarPeriodoFake()
        {
            return new PeriodoRealizacao
            {
                DataInicio = _faker.Date.Past(),
                DataFim = _faker.Date.Recent()
            };
        }

        private void SetupDependenciasDeGeracao(DadosEmissaoDeclaracaoCodafDto dto, PeriodoRealizacao? periodo, TipoEstrategiaCodaf estrategia)
        {
            var geradorMock = new Mock<IDeclaracaoCodafGeradorConteudo>();
            geradorMock
                .Setup(x => x.GerarHtml(It.IsAny<DadosEmissaoDeclaracaoCodafDto>()))
                .Returns($"<html>Declaracao {estrategia} Fake</html>");

            _mocker.GetMock<IKeyedServiceProvider>()
                .Setup(x => x.GetRequiredKeyedService(typeof(IDeclaracaoCodafGeradorConteudo), (object)estrategia))
                .Returns(geradorMock.Object);

            _mocker.GetMock<IPeriodoRealizacaoConsultaService>()
                .Setup(x => x.ObterPeriodoRealizacaoAsync(dto.PropostaTurmaId))
                .ReturnsAsync(periodo);
        }
    }
}
