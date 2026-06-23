using ConectaFormacao.Dominio.Servicos;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCertificados;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Estrategias.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoEmitirCertificadoCodafTestes
    {
        private readonly Mock<IRepositorioCodafCertificado> _mockRepositorioCodafCertificado;
        private readonly Mock<IKeyedServiceProvider> _mockServiceProvider;
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<IPeriodoRealizacaoConsultaService> _mockPeriodoRealizacaoConsultaService;
        private readonly CasoDeUsoEmitirCertificadoCodaf _sut;

        public CasoDeUsoEmitirCertificadoCodafTestes()
        {
            _mockRepositorioCodafCertificado = new Mock<IRepositorioCodafCertificado>();
            _mockServiceProvider = new Mock<IKeyedServiceProvider>();
            _mockMediator = new Mock<IMediator>();
            _mockPeriodoRealizacaoConsultaService = new Mock<IPeriodoRealizacaoConsultaService>();

            _sut = new CasoDeUsoEmitirCertificadoCodaf(
                _mockRepositorioCodafCertificado.Object,
                _mockServiceProvider.Object,
                _mockMediator.Object,
                _mockPeriodoRealizacaoConsultaService.Object);
        }

        #region Testes de Retorno - Sem Dados

        [Fact]
        public async Task ExecutarAsync_DeveRetornarErroNaoEncontrado_QuandoListaDadosCertificadoEstaVazia()
        {
            // Arrange
            const long codafListaPresencaId = 1;
            _mockRepositorioCodafCertificado
                .Setup(x => x.ObterDadosParaEmissaoCertificadosCodafAsync(codafListaPresencaId))
                .ReturnsAsync(new List<DadosEmissaoCertificadoCodafDto>());

            // Act
            var resultado = await _sut.ExecutarAsync(codafListaPresencaId);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);
            _mockRepositorioCodafCertificado.Verify(
                x => x.ObterDadosParaEmissaoCertificadosCodafAsync(codafListaPresencaId),
                Times.Once);
        }

        #endregion

        #region Testes com Cursista Sem RF

        [Fact]
        public async Task ExecutarAsync_DeveEmitirCertificadoCursistaSemRf_QuandoDadosValidosE_TemRfFalso()
        {
            // Arrange
            const long codafListaPresencaId = 1;
            const long inscricaoId = 100;
            const long propostaTurmaId = 50;

            var dadosEmissao = new DadosEmissaoCertificadoCodafDto
            {
                IdReferencia = inscricaoId,
                PropostaTurmaId = propostaTurmaId,
                NomeCompleto = "João da Silva",
                Documento = "12345678910",
                TemRf = false,
                TipoParticipacao = TipoParticipacaoCodaf.Cursista,
                NomeFormacao = "Curso .NET",
                DataRealizacao = new DateTime(2024, 1, 15),
                HorasTotais = 20,
                CargaHorariaTotalOutra = null,
                ConceitoFinal = "S",
                PercentualFrequencia = 100,
                NomeEmissor = "DRE 1",
                TipoFormacao = "curso",
                DataInicio = DateTime.MinValue,
                DataFim = DateTime.MinValue,
                EmailUsuario = "joao@example.com"
            };

            var periodo = new PeriodoRealizacao
            {
                DataInicio = new DateTime(2024, 1, 10),
                DataFim = new DateTime(2024, 1, 20)
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
            resultado.TipoFalha.Should().Be(TipoFalha.Nenhuma);

            mockGerador.Verify(
                x => x.GerarHtml(It.Is<DadosEmissaoCertificadoCodafDto>(d =>
                    d.IdReferencia == inscricaoId &&
                    d.DataInicio == periodo.DataInicio &&
                    d.DataFim == periodo.DataFim)),
                Times.Once);

            _mockRepositorioCodafCertificado.Verify(
                x => x.InserirLoteAsync(It.Is<IEnumerable<CodafCertificado>>(lista =>
                    lista.Count() == 1 &&
                    lista.First().TipoParticipacao == TipoParticipacaoCodaf.Cursista)),
                Times.Once);

            _mockMediator.Verify(
                x => x.Send(
                    It.Is<PublicarNaFilaRabbitCommand>(cmd =>
                        cmd.Rota == RotasRabbit.GerarArquivoCertificadosCodaf &&
                        (long)cmd.Filtros == codafListaPresencaId),
                    It.IsAny<CancellationToken>()),
                Times.Once);
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
                NomeFormacao = "Formação Docente",
                DataRealizacao = new DateTime(2024, 2, 10),
                HorasTotais = 30,
                CargaHorariaTotalOutra = null,
                ConceitoFinal = "S",
                PercentualFrequencia = 95,
                NomeEmissor = "DRE 2",
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
                NomeEmissor = "Coordenadoria Centro",
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
                .Returns("<html>Certificado Regente</html>");

            _mockRepositorioCodafCertificado
                .Setup(x => x.ObterDadosParaEmissaoCertificadosCodafAsync(codafListaPresencaId))
                .ReturnsAsync(new List<DadosEmissaoCertificadoCodafDto> { dadosEmissao });

            _mockPeriodoRealizacaoConsultaService
                .Setup(x => x.ObterPeriodoRealizacaoAsync(propostaTurmaId))
                .ReturnsAsync(periodo);

            _mockServiceProvider
                .Setup(x => x.GetRequiredKeyedService(typeof(ICertificadoCodafGeradorConteudo), (object)TipoEstrategiaCertificadoCodaf.Regente))
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

        #region Testes com Período Nulo

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
                NomeFormacao = "Treinamento Rápido",
                DataRealizacao = new DateTime(2024, 4, 1),
                HorasTotais = 5,
                CargaHorariaTotalOutra = null,
                ConceitoFinal = "S",
                PercentualFrequencia = 100,
                NomeEmissor = "DRE 3",
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

        #region Testes com Múltiplos Certificados

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
                NomeEmissor = "DRE 1",
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
                NomeEmissor = "DRE 2",
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
                NomeEmissor = "Coordenadoria",
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
                .Returns("<html>Certificado Regente</html>");

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
                .Setup(x => x.GetRequiredKeyedService(typeof(ICertificadoCodafGeradorConteudo), (object)TipoEstrategiaCertificadoCodaf.Regente))
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
                NomeEmissor = "DRE Teste",
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

        #region Testes de Interações com Repositório

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

        #region Testes com Carga Horária Alternativa

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
                NomeEmissor = "DRE Alternativa",
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