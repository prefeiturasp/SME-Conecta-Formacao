using Bogus;
using ConectaFormacao.Dominio.Servicos;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCertificados;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Estrategias.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.CodafCertificados
{
    public class CasoDeUsoEmitirCertificadoCodafTestes
    {
        private readonly CasoDeUsoEmitirCertificadoCodaf _sut;
        private readonly Faker _faker;
        private readonly AutoMocker _mocker;

        private readonly Mock<IRepositorioCodafCertificado> _repositorioCodafCertificadoMock;
        private readonly Mock<IKeyedServiceProvider> _serviceProviderMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IPeriodoRealizacaoConsultaService> _periodoConsultaMock;
        private readonly Mock<IRepositorioCodafSuplementarInscricao> _repositorioSuplementarInscricaoMock;
        private readonly Mock<ITransacao> _transacaoMock;
        private readonly Mock<IDbTransaction> _dbTransactionMock;

        public CasoDeUsoEmitirCertificadoCodafTestes()
        {
            _mocker = new AutoMocker();
            _faker = new Faker("pt_BR");

            _repositorioCodafCertificadoMock = _mocker.GetMock<IRepositorioCodafCertificado>();
            _serviceProviderMock = _mocker.GetMock<IKeyedServiceProvider>();
            _mediatorMock = _mocker.GetMock<IMediator>();
            _periodoConsultaMock = _mocker.GetMock<IPeriodoRealizacaoConsultaService>();
            _repositorioSuplementarInscricaoMock = _mocker.GetMock<IRepositorioCodafSuplementarInscricao>();
            _transacaoMock = _mocker.GetMock<ITransacao>();

            _dbTransactionMock = new Mock<IDbTransaction>();
            _transacaoMock.Setup(t => t.Iniciar()).Returns(_dbTransactionMock.Object);

            _sut = _mocker.CreateInstance<CasoDeUsoEmitirCertificadoCodaf>();
        }

        [Fact]
        public async Task DadoTipoCodafInvalido_QuandoExecutarAsync_EntaoDeveLancarArgumentOutOfRangeException()
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
        public async Task DadoNenhumDadoParaEmissao_QuandoExecutarAsync_EntaoDeveRetornarErroNaoEncontrado()
        {
            // Arrange
            var codafId = _faker.Random.Long(1, 100);

            _repositorioCodafCertificadoMock
                .Setup(x => x.ObterDadosParaEmissaoCertificadosCodafAsync(codafId))
                .ReturnsAsync(new List<DadosEmissaoCertificadoCodafDto>());

            // Act
            var resultado = await _sut.ExecutarAsync(codafId, TipoCodaf.ListaPresenca);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);
            _transacaoMock.Verify(t => t.Iniciar(), Times.Never);
        }

        [Fact]
        public async Task DadoCodafListaPresencaECursistaSemRf_QuandoExecutarAsync_EntaoDeveEmitirSemSanitizacaoEComitar()
        {
            // Arrange
            var codafId = _faker.Random.Long(1, 100);
            var dadosEmissao = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Cursista, temRf: false);
            var periodo = CriarPeriodoFake();

            SetupDependenciasDeGeracao(dadosEmissao, periodo, TipoEstrategiaCertificadoCodaf.CursistaSemRf);

            _repositorioCodafCertificadoMock
                .Setup(x => x.ObterDadosParaEmissaoCertificadosCodafAsync(codafId))
                .ReturnsAsync([dadosEmissao]);

            // Act
            var resultado = await _sut.ExecutarAsync(codafId, TipoCodaf.ListaPresenca);

            // Assert
            resultado.Sucesso.Should().BeTrue();

            _repositorioCodafCertificadoMock.Verify(x => x.InativarCertificadosAnterioresCursistaAsync(It.IsAny<IEnumerable<long>>()), Times.Never);
            _repositorioCodafCertificadoMock.Verify(x => x.InserirLoteAsync(It.Is<IEnumerable<CodafCertificado>>(l => l.Count() == 1)), Times.Once);
            _repositorioCodafCertificadoMock.Verify(x => x.AtualizaCodigoCertificado(codafId, TipoCodaf.ListaPresenca), Times.Once);
            _dbTransactionMock.Verify(t => t.Commit(), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.Is<PublicarNaFilaRabbitCommand>(cmd => cmd.Rota == RotasRabbit.GerarArquivoCertificadosCodaf), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoCodafSuplementarECursistaComRf_QuandoExecutarAsync_EntaoDeveSanitizarInserirEComitar()
        {
            // Arrange
            var codafId = _faker.Random.Long(1, 100);
            var dadosEmissao = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Cursista, temRf: true);
            var periodo = CriarPeriodoFake();
            var inscritosReprovados = new List<long> { _faker.Random.Long(1000, 2000) };

            SetupDependenciasDeGeracao(dadosEmissao, periodo, TipoEstrategiaCertificadoCodaf.CursistaComRf);

            _repositorioCodafCertificadoMock
                .Setup(x => x.ObterDadosParaEmissaoCertificadosCodafSuplementarAsync(codafId))
                .ReturnsAsync([dadosEmissao]);

            _repositorioSuplementarInscricaoMock
                .Setup(x => x.ObterIdInscritosReprovadosAsync(codafId))
                .ReturnsAsync(inscritosReprovados);

            // Act
            var resultado = await _sut.ExecutarAsync(codafId, TipoCodaf.Suplementar);

            // Assert
            resultado.Sucesso.Should().BeTrue();

            var idsEsperadosParaSanitizacao = new List<long> { dadosEmissao.InscricaoId }.Union(inscritosReprovados);

            _repositorioCodafCertificadoMock.Verify(x => x.InativarCertificadosAnterioresCursistaAsync(It.Is<IEnumerable<long>>(ids => ids.SequenceEqual(idsEsperadosParaSanitizacao))), Times.Once);
            _repositorioCodafCertificadoMock.Verify(x => x.InserirLoteAsync(It.IsAny<IEnumerable<CodafCertificado>>()), Times.Once);
            _repositorioCodafCertificadoMock.Verify(x => x.AtualizaCodigoCertificado(codafId, TipoCodaf.Suplementar), Times.Once);
            _dbTransactionMock.Verify(t => t.Commit(), Times.Once);
        }

        [Fact]
        public async Task DadoExcecaoNaInsercao_QuandoExecutarAsync_EntaoDeveFazerRollbackELancarExcecao()
        {
            // Arrange
            var codafId = _faker.Random.Long(1, 100);
            var dadosEmissao = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Regente, temRf: true);
            var periodo = CriarPeriodoFake();

            SetupDependenciasDeGeracao(dadosEmissao, periodo, TipoEstrategiaCertificadoCodaf.Regente);

            _repositorioCodafCertificadoMock
                .Setup(x => x.ObterDadosParaEmissaoCertificadosCodafAsync(codafId))
                .ReturnsAsync([dadosEmissao]);

            _repositorioCodafCertificadoMock
                .Setup(x => x.InserirLoteAsync(It.IsAny<IEnumerable<CodafCertificado>>()))
                .ThrowsAsync(new Exception("Erro no banco de dados simulado."));

            // Act
            var acao = async () => await _sut.ExecutarAsync(codafId, TipoCodaf.ListaPresenca);

            // Assert
            await acao.Should().ThrowAsync<Exception>().WithMessage("Erro no banco de dados simulado.");
            _dbTransactionMock.Verify(t => t.Rollback(), Times.Once);
            _dbTransactionMock.Verify(t => t.Commit(), Times.Never);
        }

        [Fact]
        public async Task DadoEmissaoDeRegente_QuandoExecutarAsync_EntaoDeveUtilizarEstrategiaCorreta()
        {
            // Arrange
            var codafId = _faker.Random.Long(1, 100);
            var dadosEmissao = CriarDadosEmissaoDtoFake(TipoParticipacaoCodaf.Regente, temRf: true);

            SetupDependenciasDeGeracao(dadosEmissao, null, TipoEstrategiaCertificadoCodaf.Regente);

            _repositorioCodafCertificadoMock
                .Setup(x => x.ObterDadosParaEmissaoCertificadosCodafAsync(codafId))
                .ReturnsAsync([dadosEmissao]);

            // Act
            var resultado = await _sut.ExecutarAsync(codafId, TipoCodaf.ListaPresenca);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _serviceProviderMock.Verify(x => x.GetRequiredKeyedService(typeof(ICertificadoCodafGeradorConteudo), (object)TipoEstrategiaCertificadoCodaf.Regente), Times.Once);
            _dbTransactionMock.Verify(t => t.Commit(), Times.Once);
        }

        // --- MÉTODOS DE APOIO (DRY & KISS) ---

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
                NomeFormacao = "Formação Fake",
                DataRealizacao = _faker.Date.Recent(),
                HorasTotais = _faker.Random.Int(10, 40),
                Emissor = "DRE Local",
                TipoFormacao = "curso"
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

        private void SetupDependenciasDeGeracao(DadosEmissaoCertificadoCodafDto dto, PeriodoRealizacao? periodo, TipoEstrategiaCertificadoCodaf estrategia)
        {
            var geradorMock = new Mock<ICertificadoCodafGeradorConteudo>();
            geradorMock
                .Setup(x => x.GerarHtml(It.IsAny<DadosEmissaoCertificadoCodafDto>()))
                .Returns("<html>Certificado Fake</html>");

            _serviceProviderMock
                .Setup(x => x.GetRequiredKeyedService(typeof(ICertificadoCodafGeradorConteudo), (object)estrategia))
                .Returns(geradorMock.Object);

            _periodoConsultaMock
                .Setup(x => x.ObterPeriodoRealizacaoAsync(dto.PropostaTurmaId))
                .ReturnsAsync(periodo);
        }
    }
}