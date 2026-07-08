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

        // --- M�TODOS DE APOIO (DRY & KISS) ---

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
                NomeFormacao = "Forma��o Fake",
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

        #endregion

        #region Testes com Cursista Com RF

        [Fact]
        public async Task ExecutarAsync_DeveEmitirCertificadoCursistaComRf_QuandoDadosValidosE_TemRfVerdadeiro()
        {
            // Arrange
            const long codafListaPresencaId = 2;
            const long inscricaoId = 101;
            const long propostaTurmaId = 51;

            var dadosEmissao = new DadosEmissaoCertificadoCodafDto
            {
                IdReferencia = inscricaoId,
                PropostaTurmaId = propostaTurmaId,
                NomeCompleto = "Maria da Silva",
                Documento = "1234567",
                TemRf = true,
                TipoParticipacao = TipoParticipacaoCodaf.Cursista,
                NomeFormacao = "Forma��o Docente",
                DataRealizacao = new DateTime(2024, 2, 10),
                HorasTotais = 30,
                CargaHorariaTotalOutra = null,
                ConceitoFinal = "S",
                PercentualFrequencia = 95,
                Emissor = "DRE 2",
                TipoFormacao = "curso",
                DataInicio = DateTime.MinValue,
                DataFim = DateTime.MinValue,
                EmailUsuario = "maria@example.com"
            };

            var periodo = new PeriodoRealizacao
            {
                DataInicio = new DateTime(2024, 2, 5),
                DataFim = new DateTime(2024, 2, 15)
            };

            var mockGerador = new Mock<ICertificadoCodafGeradorConteudo>();
            mockGerador
                .Setup(x => x.GerarHtml(It.IsAny<DadosEmissaoCertificadoCodafDto>()))
                .Returns("<html>Certificado Com RF</html>");

            _mockRepositorioCodafCertificado
                .Setup(x => x.ObterDadosParaEmissaoCertificadosCodafAsync(codafListaPresencaId))
                .ReturnsAsync(new List<DadosEmissaoCertificadoCodafDto> { dadosEmissao });

            _mockPeriodoRealizacaoConsultaService
                .Setup(x => x.ObterPeriodoRealizacaoAsync(propostaTurmaId))
                .ReturnsAsync(periodo);

            _mockServiceProvider
                .Setup(x => x.GetRequiredKeyedService(typeof(ICertificadoCodafGeradorConteudo), (object)TipoEstrategiaCertificadoCodaf.CursistaComRf))
                .Returns(mockGerador.Object);

            _mockRepositorioCodafCertificado
                .Setup(x => x.InserirLoteAsync(It.IsAny<IEnumerable<CodafCertificado>>()))
                .Returns(Task.CompletedTask);

            _mockMediator
                .Setup(x => x.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.ExecutarAsync(codafListaPresencaId);

            // Assert
            resultado.Sucesso.Should().BeTrue();

            _mockRepositorioCodafCertificado.Verify(
                x => x.InserirLoteAsync(It.Is<IEnumerable<CodafCertificado>>(lista =>
                    lista.First().TipoParticipacao == TipoParticipacaoCodaf.Cursista)),
                Times.Once);
        }

        #endregion

        #region Testes com Regente

        [Fact]
        public async Task ExecutarAsync_DeveEmitirCertificadoRegente_QuandoTipoParticipacaoEhRegente()
        {
            // Arrange
            const long codafListaPresencaId = 3;
            const long regenteId = 200;
            const long propostaTurmaId = 52;

            var dadosEmissao = new DadosEmissaoCertificadoCodafDto
            {
                IdReferencia = regenteId,
                PropostaTurmaId = propostaTurmaId,
                NomeCompleto = "Carlos Professor",
                Documento = "9876543",
                TemRf = true,
                TipoParticipacao = TipoParticipacaoCodaf.Regente,
                NomeFormacao = "Workshop Pedagogia",
                DataRealizacao = new DateTime(2024, 3, 20),
                HorasTotais = 40,
                CargaHorariaTotalOutra = null,
                ConceitoFinal = null,
                PercentualFrequencia = null,
                Emissor = "Coordenadoria Centro",
                TipoFormacao = "evento",
                DataInicio = DateTime.MinValue,
                DataFim = DateTime.MinValue,
                EmailUsuario = "carlos@example.com"
            };

            var periodo = new PeriodoRealizacao
            {
                DataInicio = new DateTime(2024, 3, 15),
                DataFim = new DateTime(2024, 3, 25)
            };

            var mockGerador = new Mock<ICertificadoCodafGeradorConteudo>();
            mockGerador
                .Setup(x => x.GerarHtml(It.IsAny<DadosEmissaoCertificadoCodafDto>()))
                .Returns("<html>Certificado RegenteComRf</html>");

            _mockRepositorioCodafCertificado
                .Setup(x => x.ObterDadosParaEmissaoCertificadosCodafAsync(codafListaPresencaId))
                .ReturnsAsync(new List<DadosEmissaoCertificadoCodafDto> { dadosEmissao });

            _mockPeriodoRealizacaoConsultaService
                .Setup(x => x.ObterPeriodoRealizacaoAsync(propostaTurmaId))
                .ReturnsAsync(periodo);

            _mockServiceProvider
                .Setup(x => x.GetRequiredKeyedService(typeof(ICertificadoCodafGeradorConteudo), (object)TipoEstrategiaCertificadoCodaf.RegenteComRf))
                .Returns(mockGerador.Object);

            _mockRepositorioCodafCertificado
                .Setup(x => x.InserirLoteAsync(It.IsAny<IEnumerable<CodafCertificado>>()))
                .Returns(Task.CompletedTask);

            _mockMediator
                .Setup(x => x.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.ExecutarAsync(codafListaPresencaId);

            // Assert
            resultado.Sucesso.Should().BeTrue();

            _mockRepositorioCodafCertificado.Verify(
                x => x.InserirLoteAsync(It.Is<IEnumerable<CodafCertificado>>(lista =>
                    lista.First().TipoParticipacao == TipoParticipacaoCodaf.Regente)),
                Times.Once);
        }

        #endregion

        #region Testes com Per�odo Nulo

        [Fact]
        public async Task ExecutarAsync_DeveProcessarComPeriodoNulo_QuandoServicoRetornaNulo()
        {
            // Arrange
            const long codafListaPresencaId = 4;
            const long inscricaoId = 102;
            const long propostaTurmaId = 53;

            var dadosEmissao = new DadosEmissaoCertificadoCodafDto
            {
                IdReferencia = inscricaoId,
                PropostaTurmaId = propostaTurmaId,
                NomeCompleto = "Pedro Silva",
                Documento = "11111111111",
                TemRf = false,
                TipoParticipacao = TipoParticipacaoCodaf.Cursista,
                NomeFormacao = "Treinamento R�pido",
                DataRealizacao = new DateTime(2024, 4, 1),
                HorasTotais = 5,
                CargaHorariaTotalOutra = null,
                ConceitoFinal = "S",
                PercentualFrequencia = 100,
                Emissor = "DRE 3",
                TipoFormacao = "curso",
                DataInicio = DateTime.MinValue,
                DataFim = DateTime.MinValue,
                EmailUsuario = "pedro@example.com"
            };

            var mockGerador = new Mock<ICertificadoCodafGeradorConteudo>();
            mockGerador
                .Setup(x => x.GerarHtml(It.IsAny<DadosEmissaoCertificadoCodafDto>()))
                .Returns("<html>Certificado</html>");

            _mockRepositorioCodafCertificado
                .Setup(x => x.ObterDadosParaEmissaoCertificadosCodafAsync(codafListaPresencaId))
                .ReturnsAsync(new List<DadosEmissaoCertificadoCodafDto> { dadosEmissao });

            _mockPeriodoRealizacaoConsultaService
                .Setup(x => x.ObterPeriodoRealizacaoAsync(propostaTurmaId))
                .ReturnsAsync((PeriodoRealizacao?)null);

            _mockServiceProvider
                .Setup(x => x.GetRequiredKeyedService(typeof(ICertificadoCodafGeradorConteudo), (object)TipoEstrategiaCertificadoCodaf.CursistaSemRf))
                .Returns(mockGerador.Object);

            _mockRepositorioCodafCertificado
                .Setup(x => x.InserirLoteAsync(It.IsAny<IEnumerable<CodafCertificado>>()))
                .Returns(Task.CompletedTask);

            _mockMediator
                .Setup(x => x.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.ExecutarAsync(codafListaPresencaId);

            // Assert
            resultado.Sucesso.Should().BeTrue();

            mockGerador.Verify(
                x => x.GerarHtml(It.Is<DadosEmissaoCertificadoCodafDto>(d =>
                    d.DataInicio == DateTime.MinValue &&
                    d.DataFim == DateTime.MinValue)),
                Times.Once);

            _mockRepositorioCodafCertificado.Verify(
                x => x.InserirLoteAsync(It.IsAny<IEnumerable<CodafCertificado>>()),
                Times.Once);
        }

        #endregion

        #region Testes com M�ltiplos Certificados

        [Fact]
        public async Task ExecutarAsync_DeveProcessarMultiplosCertificados_ComTiposEstrategiasDiferentes()
        {
            // Arrange
            const long codafListaPresencaId = 5;

            var dadosCursistaSemRf = new DadosEmissaoCertificadoCodafDto
            {
                IdReferencia = 103,
                PropostaTurmaId = 54,
                NomeCompleto = "Ana Silva",
                Documento = "22222222222",
                TemRf = false,
                TipoParticipacao = TipoParticipacaoCodaf.Cursista,
                NomeFormacao = "Curso 1",
                DataRealizacao = new DateTime(2024, 5, 1),
                HorasTotais = 20,
                CargaHorariaTotalOutra = null,
                ConceitoFinal = "S",
                PercentualFrequencia = 100,
                Emissor = "DRE 1",
                TipoFormacao = "curso",
                DataInicio = DateTime.MinValue,
                DataFim = DateTime.MinValue,
                EmailUsuario = "ana@example.com"
            };

            var dadosCursistaComRf = new DadosEmissaoCertificadoCodafDto
            {
                IdReferencia = 104,
                PropostaTurmaId = 55,
                NomeCompleto = "Bruno Santos",
                Documento = "3333333",
                TemRf = true,
                TipoParticipacao = TipoParticipacaoCodaf.Cursista,
                NomeFormacao = "Curso 2",
                DataRealizacao = new DateTime(2024, 5, 2),
                HorasTotais = 30,
                CargaHorariaTotalOutra = null,
                ConceitoFinal = "S",
                PercentualFrequencia = 95,
                Emissor = "DRE 2",
                TipoFormacao = "curso",
                DataInicio = DateTime.MinValue,
                DataFim = DateTime.MinValue,
                EmailUsuario = "bruno@example.com"
            };

            var dadosRegente = new DadosEmissaoCertificadoCodafDto
            {
                IdReferencia = 105,
                PropostaTurmaId = 56,
                NomeCompleto = "Diana Prof",
                Documento = "4444444",
                TemRf = true,
                TipoParticipacao = TipoParticipacaoCodaf.Regente,
                NomeFormacao = "Curso 3",
                DataRealizacao = new DateTime(2024, 5, 3),
                HorasTotais = 40,
                CargaHorariaTotalOutra = null,
                ConceitoFinal = null,
                PercentualFrequencia = null,
                Emissor = "Coordenadoria",
                TipoFormacao = "evento",
                DataInicio = DateTime.MinValue,
                DataFim = DateTime.MinValue,
                EmailUsuario = "diana@example.com"
            };

            var mockGeradorCursistaSemRf = new Mock<ICertificadoCodafGeradorConteudo>();
            var mockGeradorCursistaComRf = new Mock<ICertificadoCodafGeradorConteudo>();
            var mockGeradorRegente = new Mock<ICertificadoCodafGeradorConteudo>();

            mockGeradorCursistaSemRf
                .Setup(x => x.GerarHtml(It.IsAny<DadosEmissaoCertificadoCodafDto>()))
                .Returns("<html>Certificado Cursista Sem RF</html>");

            mockGeradorCursistaComRf
                .Setup(x => x.GerarHtml(It.IsAny<DadosEmissaoCertificadoCodafDto>()))
                .Returns("<html>Certificado Cursista Com RF</html>");

            mockGeradorRegente
                .Setup(x => x.GerarHtml(It.IsAny<DadosEmissaoCertificadoCodafDto>()))
                .Returns("<html>Certificado RegenteComRf</html>");

            _mockRepositorioCodafCertificado
                .Setup(x => x.ObterDadosParaEmissaoCertificadosCodafAsync(codafListaPresencaId))
                .ReturnsAsync(new List<DadosEmissaoCertificadoCodafDto> { dadosCursistaSemRf, dadosCursistaComRf, dadosRegente });

            _mockPeriodoRealizacaoConsultaService
                .Setup(x => x.ObterPeriodoRealizacaoAsync(It.IsAny<long>()))
                .ReturnsAsync((PeriodoRealizacao?)null);

            _mockServiceProvider
                .Setup(x => x.GetRequiredKeyedService(typeof(ICertificadoCodafGeradorConteudo), (object)TipoEstrategiaCertificadoCodaf.CursistaSemRf))
                .Returns(mockGeradorCursistaSemRf.Object);

            _mockServiceProvider
                .Setup(x => x.GetRequiredKeyedService(typeof(ICertificadoCodafGeradorConteudo), (object)TipoEstrategiaCertificadoCodaf.CursistaComRf))
                .Returns(mockGeradorCursistaComRf.Object);

            _mockServiceProvider
                .Setup(x => x.GetRequiredKeyedService(typeof(ICertificadoCodafGeradorConteudo), (object)TipoEstrategiaCertificadoCodaf.RegenteComRf))
                .Returns(mockGeradorRegente.Object);

            _mockRepositorioCodafCertificado
                .Setup(x => x.InserirLoteAsync(It.IsAny<IEnumerable<CodafCertificado>>()))
                .Returns(Task.CompletedTask);

            _mockMediator
                .Setup(x => x.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.ExecutarAsync(codafListaPresencaId);

            // Assert
            resultado.Sucesso.Should().BeTrue();

            mockGeradorCursistaSemRf.Verify(
                x => x.GerarHtml(It.IsAny<DadosEmissaoCertificadoCodafDto>()),
                Times.Once);

            mockGeradorCursistaComRf.Verify(
                x => x.GerarHtml(It.IsAny<DadosEmissaoCertificadoCodafDto>()),
                Times.Once);

            mockGeradorRegente.Verify(
                x => x.GerarHtml(It.IsAny<DadosEmissaoCertificadoCodafDto>()),
                Times.Once);

            _mockRepositorioCodafCertificado.Verify(
                x => x.InserirLoteAsync(It.Is<IEnumerable<CodafCertificado>>(lista => lista.Count() == 3)),
                Times.Once);

            _mockMediator.Verify(
                x => x.Send(
                    It.Is<PublicarNaFilaRabbitCommand>(cmd =>
                        cmd.Rota == RotasRabbit.GerarArquivoCertificadosCodaf),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #endregion

        #region Testes de Metadados

        [Fact]
        public async Task ExecutarAsync_DeveInserirCertificadoComMetadadosCorretos()
        {
            // Arrange
            const long codafListaPresencaId = 6;
            const long inscricaoId = 106;
            const long propostaTurmaId = 57;

            var dadosEmissao = new DadosEmissaoCertificadoCodafDto
            {
                IdReferencia = inscricaoId,
                PropostaTurmaId = propostaTurmaId,
                NomeCompleto = "Teste Metadados",
                Documento = "55555555555",
                TemRf = false,
                TipoParticipacao = TipoParticipacaoCodaf.Cursista,
                NomeFormacao = "Curso Metadados",
                DataRealizacao = new DateTime(2024, 6, 1),
                HorasTotais = 25,
                CargaHorariaTotalOutra = null,
                ConceitoFinal = "S",
                PercentualFrequencia = 100,
                Emissor = "DRE Teste",
                TipoFormacao = "curso",
                DataInicio = DateTime.MinValue,
                DataFim = DateTime.MinValue,
                EmailUsuario = "teste@example.com"
            };

            var periodo = new PeriodoRealizacao
            {
                DataInicio = new DateTime(2024, 6, 1),
                DataFim = new DateTime(2024, 6, 15)
            };

            var mockGerador = new Mock<ICertificadoCodafGeradorConteudo>();
            mockGerador
                .Setup(x => x.GerarHtml(It.IsAny<DadosEmissaoCertificadoCodafDto>()))
                .Returns("<html>Certificado</html>");

            _mockRepositorioCodafCertificado
                .Setup(x => x.ObterDadosParaEmissaoCertificadosCodafAsync(codafListaPresencaId))
                .ReturnsAsync(new List<DadosEmissaoCertificadoCodafDto> { dadosEmissao });

            _mockPeriodoRealizacaoConsultaService
                .Setup(x => x.ObterPeriodoRealizacaoAsync(propostaTurmaId))
                .ReturnsAsync(periodo);

            _mockServiceProvider
                .Setup(x => x.GetRequiredKeyedService(typeof(ICertificadoCodafGeradorConteudo), (object)TipoEstrategiaCertificadoCodaf.CursistaSemRf))
                .Returns(mockGerador.Object);

            _mockRepositorioCodafCertificado
                .Setup(x => x.InserirLoteAsync(It.IsAny<IEnumerable<CodafCertificado>>()))
                .Returns(Task.CompletedTask);

            _mockMediator
                .Setup(x => x.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.ExecutarAsync(codafListaPresencaId);

            // Assert
            resultado.Sucesso.Should().BeTrue();

            _mockRepositorioCodafCertificado.Verify(
                x => x.InserirLoteAsync(It.Is<IEnumerable<CodafCertificado>>(lista =>
                    lista.First().HtmlContentSnapshot == "<html>Certificado</html>" &&
                    lista.First().MetadadosJson != null)),
                Times.Once);
        }

        #endregion

        #region Testes de Intera��es com Reposit�rio

        [Fact]
        public async Task ExecutarAsync_DeveNaoInserirCertificados_QuandoListaSalvarVazia()
        {
            // Arrange
            const long codafListaPresencaId = 7;

            _mockRepositorioCodafCertificado
                .Setup(x => x.ObterDadosParaEmissaoCertificadosCodafAsync(codafListaPresencaId))
                .ReturnsAsync(new List<DadosEmissaoCertificadoCodafDto>());

            // Act
            var resultado = await _sut.ExecutarAsync(codafListaPresencaId);

            // Assert
            _mockRepositorioCodafCertificado.Verify(
                x => x.InserirLoteAsync(It.IsAny<IEnumerable<CodafCertificado>>()),
                Times.Never);

            _mockMediator.Verify(
                x => x.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        #endregion

        #region Testes com Carga Hor�ria Alternativa

        [Fact]
        public async Task ExecutarAsync_DeveProcessarCertificadoComCargaHorariaAlternativa()
        {
            // Arrange
            const long codafListaPresencaId = 8;
            const long inscricaoId = 107;
            const long propostaTurmaId = 58;

            var dadosEmissao = new DadosEmissaoCertificadoCodafDto
            {
                IdReferencia = inscricaoId,
                PropostaTurmaId = propostaTurmaId,
                NomeCompleto = "Teste Carga Alternativa",
                Documento = "66666666666",
                TemRf = false,
                TipoParticipacao = TipoParticipacaoCodaf.Cursista,
                NomeFormacao = "Curso Alternativo",
                DataRealizacao = new DateTime(2024, 7, 1),
                HorasTotais = null,
                CargaHorariaTotalOutra = "10h30m",
                ConceitoFinal = "S",
                PercentualFrequencia = 100,
                Emissor = "DRE Alternativa",
                TipoFormacao = "curso",
                DataInicio = DateTime.MinValue,
                DataFim = DateTime.MinValue,
                EmailUsuario = "alternativo@example.com"
            };

            var periodo = new PeriodoRealizacao
            {
                DataInicio = new DateTime(2024, 7, 1),
                DataFim = new DateTime(2024, 7, 10)
            };

            var mockGerador = new Mock<ICertificadoCodafGeradorConteudo>();
            mockGerador
                .Setup(x => x.GerarHtml(It.IsAny<DadosEmissaoCertificadoCodafDto>()))
                .Returns("<html>Certificado Alternativo</html>");

            _mockRepositorioCodafCertificado
                .Setup(x => x.ObterDadosParaEmissaoCertificadosCodafAsync(codafListaPresencaId))
                .ReturnsAsync(new List<DadosEmissaoCertificadoCodafDto> { dadosEmissao });

            _mockPeriodoRealizacaoConsultaService
                .Setup(x => x.ObterPeriodoRealizacaoAsync(propostaTurmaId))
                .ReturnsAsync(periodo);

            _mockServiceProvider
                .Setup(x => x.GetRequiredKeyedService(typeof(ICertificadoCodafGeradorConteudo), (object)TipoEstrategiaCertificadoCodaf.CursistaSemRf))
                .Returns(mockGerador.Object);

            _mockRepositorioCodafCertificado
                .Setup(x => x.InserirLoteAsync(It.IsAny<IEnumerable<CodafCertificado>>()))
                .Returns(Task.CompletedTask);

            _mockMediator
                .Setup(x => x.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.ExecutarAsync(codafListaPresencaId);

            // Assert
            resultado.Sucesso.Should().BeTrue();

            mockGerador.Verify(
                x => x.GerarHtml(It.Is<DadosEmissaoCertificadoCodafDto>(d =>
                    d.CargaHorariaTotalOutra == "10h30m" &&
                    d.HorasTotais == null)),
                Times.Once);
        }

        #endregion
    }
}