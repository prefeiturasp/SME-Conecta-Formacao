using Bogus;
using ConectaFormacao.Dominio.Servicos;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCertificados;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafCertificados;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Estrategias.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoEmitirCertificadoCodafTestes
    {
        private readonly ICasoDeUsoEmitirCertificadoCodaf _sut;
        private readonly Faker _faker;
        private readonly AutoMocker _mocker;
        private readonly Mock<IDbTransaction> _dbTransactionMock;

        public CasoDeUsoEmitirCertificadoCodafTestes()
        {
            _mocker = new AutoMocker();
            _faker = new Faker("pt_BR");

            _dbTransactionMock = new Mock<IDbTransaction>();
            _mocker.GetMock<ITransacao>()
                   .Setup(t => t.Iniciar())
                   .Returns(_dbTransactionMock.Object);

            _sut = _mocker.CreateInstance<CasoDeUsoEmitirCertificadoCodaf>();
        }

        [Fact]
        public async Task DadoTipoCodafInvalidoQuandoExecutarAsyncEntaoDeveLancarArgumentOutOfRangeException()
        {
            // Arrange
            var codafId = _faker.Random.Long(1, 100);
            var tipoCodafInvalido = (TipoCodaf)99;

            // Act
            var acao = async () => await _sut.ExecutarAsync(codafId, tipoCodafInvalido);

            // Assert
            await acao.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task DadoNenhumDadoParaEmissaoQuandoExecutarAsyncEntaoDeveRetornarErroNaoEncontrado()
        {
            // Arrange
            var codafId = _faker.Random.Long(1, 100);

            _mocker.GetMock<IRepositorioCodafCertificado>()
                .Setup(x => x.ObterDadosParaEmissaoCertificadosCodafAsync(codafId))
                .ReturnsAsync([]);

            // Act
            var resultado = await _sut.ExecutarAsync(codafId, TipoCodaf.ListaPresenca);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);

            _mocker.GetMock<ITransacao>().Verify(t => t.Iniciar(), Times.Never);
        }

        [Fact]
        public async Task DadoCodafListaPresencaECursistaSemRfQuandoExecutarAsyncEntaoDeveEmitirSemSanitizacaoEComitar()
        {
            // Arrange
            var codafId = _faker.Random.Long(1, 100);
            var dadosEmissao = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Cursista, temRf: false);
            var periodo = CriarPeriodoFake();

            SetupDependenciasDeGeracao(dadosEmissao, periodo, TipoEstrategiaCodaf.CursistaSemRf);

            _mocker.GetMock<IRepositorioCodafCertificado>()
                .Setup(x => x.ObterDadosParaEmissaoCertificadosCodafAsync(codafId))
                .ReturnsAsync([dadosEmissao]);

            // Act
            var resultado = await _sut.ExecutarAsync(codafId, TipoCodaf.ListaPresenca);

            // Assert
            resultado.Sucesso.Should().BeTrue();

            _mocker.GetMock<IRepositorioCodafCertificado>().Verify(x => x.InativarCertificadosAnterioresCursistaAsync(It.IsAny<IEnumerable<long>>()), Times.Never);
            _mocker.GetMock<IRepositorioCodafCertificado>().Verify(x => x.InserirLoteAsync(It.Is<IEnumerable<CodafCertificado>>(l => l.Count() == 1)), Times.Once);
            _mocker.GetMock<IRepositorioCodafCertificado>().Verify(x => x.AtualizaCodigoCertificado(codafId, TipoCodaf.ListaPresenca), Times.Once);
            _dbTransactionMock.Verify(t => t.Commit(), Times.Once);
            _mocker.GetMock<IMediator>().Verify(x => x.Send(It.Is<PublicarNaFilaRabbitCommand>(cmd => cmd.Rota == RotasRabbit.GerarArquivoCertificadosCodaf), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoCodafSuplementarECursistaComRfQuandoExecutarAsyncEntaoDeveSanitizarInserirEComitar()
        {
            // Arrange
            var codafId = _faker.Random.Long(1, 100);
            var dadosEmissao = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Cursista, temRf: true);
            var periodo = CriarPeriodoFake();
            var inscritosReprovados = new List<long> { _faker.Random.Long(1000, 2000) };

            SetupDependenciasDeGeracao(dadosEmissao, periodo, TipoEstrategiaCodaf.CursistaComRf);

            _mocker.GetMock<IRepositorioCodafCertificado>()
                .Setup(x => x.ObterDadosParaEmissaoCertificadosCodafSuplementarAsync(codafId))
                .ReturnsAsync([dadosEmissao]);

            _mocker.GetMock<IRepositorioCodafSuplementarInscricao>()
                .Setup(x => x.ObterIdInscritosReprovadosAsync(codafId))
                .ReturnsAsync(inscritosReprovados);

            // Act
            var resultado = await _sut.ExecutarAsync(codafId, TipoCodaf.Suplementar);

            // Assert
            resultado.Sucesso.Should().BeTrue();

            var idsEsperadosParaSanitizacao = new List<long> { dadosEmissao.InscricaoId }.Union(inscritosReprovados).ToList();

            _mocker.GetMock<IRepositorioCodafCertificado>().Verify(x => x.InativarCertificadosAnterioresCursistaAsync(It.Is<IEnumerable<long>>(ids => ids.SequenceEqual(idsEsperadosParaSanitizacao))), Times.Once);
            _mocker.GetMock<IRepositorioCodafCertificado>().Verify(x => x.InserirLoteAsync(It.IsAny<IEnumerable<CodafCertificado>>()), Times.Once);
            _mocker.GetMock<IRepositorioCodafCertificado>().Verify(x => x.AtualizaCodigoCertificado(codafId, TipoCodaf.Suplementar), Times.Once);
            _dbTransactionMock.Verify(t => t.Commit(), Times.Once);
        }

        [Fact]
        public async Task DadoExcecaoNaInsercaoQuandoExecutarAsyncEntaoDeveFazerRollbackELancarExcecao()
        {
            // Arrange
            var codafId = _faker.Random.Long(1, 100);
            var dadosEmissao = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Regente, temRf: true);
            var periodo = CriarPeriodoFake();

            SetupDependenciasDeGeracao(dadosEmissao, periodo, TipoEstrategiaCodaf.RegenteComRf);

            _mocker.GetMock<IRepositorioCodafCertificado>()
                .Setup(x => x.ObterDadosParaEmissaoCertificadosCodafAsync(codafId))
                .ReturnsAsync([dadosEmissao]);

            _mocker.GetMock<IRepositorioCodafCertificado>()
                .Setup(x => x.InserirLoteAsync(It.IsAny<IEnumerable<CodafCertificado>>()))
                .ThrowsAsync(new Exception("Erro de banco de dados simulado"));

            // Act
            var acao = async () => await _sut.ExecutarAsync(codafId, TipoCodaf.ListaPresenca);

            // Assert
            await acao.Should().ThrowAsync<Exception>().WithMessage("Erro de banco de dados simulado");
            _dbTransactionMock.Verify(t => t.Rollback(), Times.Once);
            _dbTransactionMock.Verify(t => t.Commit(), Times.Never);
        }

        [Fact]
        public async Task DadoEmissaoDeRegenteComRfQuandoExecutarAsyncEntaoDeveUtilizarEstrategiaRegenteComRf()
        {
            // Arrange
            var codafId = _faker.Random.Long(1, 100);
            var dadosEmissao = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Regente, temRf: true);

            SetupDependenciasDeGeracao(dadosEmissao, null, TipoEstrategiaCodaf.RegenteComRf);

            _mocker.GetMock<IRepositorioCodafCertificado>()
                .Setup(x => x.ObterDadosParaEmissaoCertificadosCodafAsync(codafId))
                .ReturnsAsync([dadosEmissao]);

            // Act
            var resultado = await _sut.ExecutarAsync(codafId, TipoCodaf.ListaPresenca);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _mocker.GetMock<IKeyedServiceProvider>().Verify(x => x.GetRequiredKeyedService(typeof(ICertificadoCodafGeradorConteudo), (object)TipoEstrategiaCodaf.RegenteComRf), Times.Once);
            _dbTransactionMock.Verify(t => t.Commit(), Times.Once);
        }

        [Fact]
        public async Task DadoEmissaoDeRegenteSemRfQuandoExecutarAsyncEntaoDeveUtilizarEstrategiaRegenteSemRf()
        {
            // Arrange
            var codafId = _faker.Random.Long(1, 100);
            var dadosEmissao = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Regente, temRf: false);

            SetupDependenciasDeGeracao(dadosEmissao, null, TipoEstrategiaCodaf.RegenteSemRf);

            _mocker.GetMock<IRepositorioCodafCertificado>()
                .Setup(x => x.ObterDadosParaEmissaoCertificadosCodafAsync(codafId))
                .ReturnsAsync([dadosEmissao]);

            // Act
            var resultado = await _sut.ExecutarAsync(codafId, TipoCodaf.ListaPresenca);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _mocker.GetMock<IKeyedServiceProvider>().Verify(x => x.GetRequiredKeyedService(typeof(ICertificadoCodafGeradorConteudo), (object)TipoEstrategiaCodaf.RegenteSemRf), Times.Once);
            _dbTransactionMock.Verify(t => t.Commit(), Times.Once);
        }

        [Fact]
        public async Task DadoPeriodoRealizacaoNuloQuandoExecutarAsyncEntaoDeveProcessarComDatasMinimas()
        {
            // Arrange
            var codafId = _faker.Random.Long(1, 100);
            var dadosEmissao = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Cursista, temRf: false);

            // Força nulo para testar fluxo
            SetupDependenciasDeGeracao(dadosEmissao, null, TipoEstrategiaCodaf.CursistaSemRf);

            _mocker.GetMock<IRepositorioCodafCertificado>()
                .Setup(x => x.ObterDadosParaEmissaoCertificadosCodafAsync(codafId))
                .ReturnsAsync([dadosEmissao]);

            // Act
            var resultado = await _sut.ExecutarAsync(codafId, TipoCodaf.ListaPresenca);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _mocker.GetMock<IRepositorioCodafCertificado>().Verify(
                x => x.InserirLoteAsync(It.Is<IEnumerable<CodafCertificado>>(lista =>
                    lista.First().MetadadosJson.Contains("\"DataInicio\":null") &&
                    lista.First().MetadadosJson.Contains("\"DataFim\":null"))),
                Times.Once);
        }

        [Fact]
        public async Task DadoCargaHorariaAlternativaQuandoExecutarAsyncEntaoDeveGerarMetadadosCorretos()
        {
            // Arrange
            var codafId = _faker.Random.Long(1, 100);
            var dadosEmissao = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Cursista, temRf: false);
            dadosEmissao.HorasTotais = null;
            dadosEmissao.CargaHorariaTotalOutra = "10h30m";

            SetupDependenciasDeGeracao(dadosEmissao, CriarPeriodoFake(), TipoEstrategiaCodaf.CursistaSemRf);

            _mocker.GetMock<IRepositorioCodafCertificado>()
                .Setup(x => x.ObterDadosParaEmissaoCertificadosCodafAsync(codafId))
                .ReturnsAsync([dadosEmissao]);

            // Act
            var resultado = await _sut.ExecutarAsync(codafId, TipoCodaf.ListaPresenca);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _mocker.GetMock<IRepositorioCodafCertificado>().Verify(
                x => x.InserirLoteAsync(It.Is<IEnumerable<CodafCertificado>>(lista =>
                    lista.First().MetadadosJson.Contains("\"HorasTotais\":null") &&
                    lista.First().MetadadosJson.Contains("\"CargaHorariaTotalOutra\":\"10h30m\""))),
                Times.Once);
        }

        [Fact]
        public async Task DadoMultiplosParticipantesQuandoExecutarAsyncEntaoDeveUtilizarEstrategiasCorrespondentes()
        {
            // Arrange
            var codafId = _faker.Random.Long(1, 100);

            var dadosCursistaSemRf = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Cursista, false);
            var dadosCursistaComRf = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Cursista, true);
            var dadosRegenteComRf = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Regente, true);

            var periodo = CriarPeriodoFake();

            SetupDependenciasDeGeracao(dadosCursistaSemRf, periodo, TipoEstrategiaCodaf.CursistaSemRf);
            SetupDependenciasDeGeracao(dadosCursistaComRf, periodo, TipoEstrategiaCodaf.CursistaComRf);
            SetupDependenciasDeGeracao(dadosRegenteComRf, periodo, TipoEstrategiaCodaf.RegenteComRf);

            _mocker.GetMock<IRepositorioCodafCertificado>()
                .Setup(x => x.ObterDadosParaEmissaoCertificadosCodafAsync(codafId))
                .ReturnsAsync([dadosCursistaSemRf, dadosCursistaComRf, dadosRegenteComRf]);

            // Act
            var resultado = await _sut.ExecutarAsync(codafId, TipoCodaf.ListaPresenca);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _mocker.GetMock<IRepositorioCodafCertificado>().Verify(x => x.InserirLoteAsync(It.Is<IEnumerable<CodafCertificado>>(lista => lista.Count() == 3)), Times.Once);

            _mocker.GetMock<IKeyedServiceProvider>().Verify(x => x.GetRequiredKeyedService(typeof(ICertificadoCodafGeradorConteudo), (object)TipoEstrategiaCodaf.CursistaSemRf), Times.Once);
            _mocker.GetMock<IKeyedServiceProvider>().Verify(x => x.GetRequiredKeyedService(typeof(ICertificadoCodafGeradorConteudo), (object)TipoEstrategiaCodaf.CursistaComRf), Times.Once);
            _mocker.GetMock<IKeyedServiceProvider>().Verify(x => x.GetRequiredKeyedService(typeof(ICertificadoCodafGeradorConteudo), (object)TipoEstrategiaCodaf.RegenteComRf), Times.Once);
        }

        private DadosEmissaoCertificadoCodafDto CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf tipoParticipacao, bool temRf)
        {
            return new DadosEmissaoCertificadoCodafDto
            {
                IdReferencia = _faker.Random.Long(1, 100),
                InscricaoId = _faker.Random.Long(101, 200),
                PropostaTurmaId = _faker.Random.Long(201, 300),
                NomeCompleto = _faker.Person.FullName,
                Documento = _faker.Random.String2(11, "0123456789"),
                TemRf = temRf,
                TipoParticipacao = tipoParticipacao,
                NomeFormacao = "Formacao Fake",
                DataRealizacao = _faker.Date.Recent(),
                HorasTotais = _faker.Random.Int(10, 40),
                Emissor = "DRE Local",
                TipoFormacao = "curso",
                ConceitoFinal = "S",
                PercentualFrequencia = 100,
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

        private void SetupDependenciasDeGeracao(DadosEmissaoCertificadoCodafDto dto, PeriodoRealizacao? periodo, TipoEstrategiaCodaf estrategia)
        {
            var geradorMock = new Mock<ICertificadoCodafGeradorConteudo>();
            geradorMock
                .Setup(x => x.GerarHtml(It.IsAny<DadosEmissaoCertificadoCodafDto>()))
                .Returns($"<html>Certificado {estrategia} Fake</html>");

            _mocker.GetMock<IKeyedServiceProvider>()
                .Setup(x => x.GetRequiredKeyedService(typeof(ICertificadoCodafGeradorConteudo), (object)estrategia))
                .Returns(geradorMock.Object);

            _mocker.GetMock<IPeriodoRealizacaoConsultaService>()
                .Setup(x => x.ObterPeriodoRealizacaoAsync(dto.PropostaTurmaId))
                .ReturnsAsync(periodo);
        }
    }
}