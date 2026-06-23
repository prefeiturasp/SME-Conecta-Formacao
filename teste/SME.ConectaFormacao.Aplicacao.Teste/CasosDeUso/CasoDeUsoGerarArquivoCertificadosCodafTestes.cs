using Bogus;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCertificados;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Estrategias.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using SME.ConectaFormacao.Infra.Servicos.Relatorio;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoGerarArquivoCertificadosCodafTestes
    {
        private readonly Mock<IServicoRelatorio> _mockServicoRelatorio;
        private readonly Mock<IRepositorioCodafCertificado> _mockRepositorioCertificado;
        private readonly Mock<IServicoArmazenamento> _mockServicoArmazenamento;
        private readonly Mock<IKeyedServiceProvider> _mockServiceProvider;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IMediator> _mockMediator;
        private readonly CasoDeUsoGerarArquivoCertificadosCodaf _sut;
        private readonly Faker _faker;

        public CasoDeUsoGerarArquivoCertificadosCodafTestes()
        {
            var mocker = new AutoMocker();
            _mockServicoRelatorio = mocker.GetMock<IServicoRelatorio>();
            _mockRepositorioCertificado = mocker.GetMock<IRepositorioCodafCertificado>();
            _mockServicoArmazenamento = mocker.GetMock<IServicoArmazenamento>();
            _mockServiceProvider = mocker.GetMock<IKeyedServiceProvider>();
            _mockConfiguration = mocker.GetMock<IConfiguration>();
            _mockMediator = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoGerarArquivoCertificadosCodaf>();
            _faker = new();
        }

        [Fact]
        public async Task DadoQueNaoTemCertificadosParaProcessar_QuandoExecutar_EntaoNaoDeveExecutarProcesso()
        {
            _mockConfiguration.Setup(x => x["UrlFrontEnd"]).Returns(_faker.Internet.Url());
            _mockRepositorioCertificado.Setup(r => r.ObterCertificadosParaProcessamentoAsync())
                .ReturnsAsync([]);

            var resultado = await _sut.Executar(new MensagemRabbit());

            resultado.Should().BeTrue();
            _mockRepositorioCertificado.Verify(x => x.ObterCertificadosParaProcessamentoAsync(), Times.Once);
            _mockServicoRelatorio.Verify(x => x.ConveterHtmlCertificadoCodafParaPdfAsync(It.IsAny<HtmlCertificadoCodafDto>()), Times.Never);
            _mockMediator.Verify(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoLoteComCertificado_QuandoProcessar_ComSucesso_DeveGerarPdf_EnviarArquivo_AtualizarStatusEEnviarEmail()
        {
            var urlFront = _faker.Internet.Url();
            _mockConfiguration.Setup(x => x["UrlFrontEnd"]).Returns(urlFront);

            var certificado = new DadosProcessamentoCertificadoCodafDto
            {
                Id = 1,
                CodigoCertificado = 12345,
                HtmlContentSnapshot = "<div>{{NUM_SEQ}} - {{NOME_EMISSOR}}</div>",
                NomeCompleto = "Fulano da Silva",
                EmailUsuario = "fulano@exemplo.com",
                NomeFormacao = "Curso X",
                TemRf = true,
                TipoParticipacao = TipoParticipacaoCodaf.Cursista,
                Emissor = "DRE-ABC"
            };

            _mockRepositorioCertificado.SetupSequence(r => r.ObterCertificadosParaProcessamentoAsync())
                .ReturnsAsync([certificado])
                .ReturnsAsync([]);

            byte[] retornoPdf = [0x1, 0x2];
            string chaveArmazenamento = "certificados/2026/12345-abc.pdf";

            string htmlRecebido = null!;
            _mockServicoRelatorio
                .Setup(s => s.ConveterHtmlCertificadoCodafParaPdfAsync(It.IsAny<HtmlCertificadoCodafDto>()))
                .Callback<HtmlCertificadoCodafDto>(h => htmlRecebido = h.HtmlContent)
                .ReturnsAsync(retornoPdf);

            _mockServicoArmazenamento
                .Setup(a => a.UploadCertificadoCodafAsync(It.IsAny<string>(), retornoPdf))
                .ReturnsAsync(chaveArmazenamento);

            var mockGerador = new Mock<ICertificadoCodafGeradorConteudo>();
            mockGerador.Setup(g => g.GerarConteudoEmail(It.IsAny<DadosProcessamentoCertificadoCodafDto>(), It.IsAny<string>()))
                .Returns(("TITULO", "CORPO"));
            _mockServiceProvider
                .Setup(sp => sp.GetRequiredKeyedService(typeof(ICertificadoCodafGeradorConteudo), It.IsAny<object>()))
                .Returns(mockGerador.Object);

            _mockMediator
                .Setup(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var resultado = await _sut.Executar(new MensagemRabbit());

            await Task.Delay(50);

            resultado.Should().BeTrue();
            _mockRepositorioCertificado.Verify(r => r.ObterCertificadosParaProcessamentoAsync(), Times.Exactly(2));
            _mockServicoRelatorio.Verify(s => s.ConveterHtmlCertificadoCodafParaPdfAsync(It.IsAny<HtmlCertificadoCodafDto>()), Times.Once);
            htmlRecebido.Should().Contain(certificado.CodigoCertificado.ToString())
                               .And.Contain(certificado.Emissor);
            _mockServicoArmazenamento.Verify(a => a.UploadCertificadoCodafAsync(It.IsAny<string>(), retornoPdf), Times.Once);
            _mockRepositorioCertificado.Verify(r => r.AtualizarStatusProcessamentoAsync(certificado.Id,
                StatusProcessamentoCertificadoCodaf.ProcessadoComSucesso, It.IsAny<string>(), null), Times.Once);

            _mockMediator.Verify(m => m.Send(It.Is<PublicarNaFilaRabbitCommand>(c => c.Rota == RotasRabbit.EnviarEmail && c.Filtros is EnviarEmailDto), It.IsAny<CancellationToken>()), Times.Once);

            _mockServiceProvider.Verify(sp => sp.GetRequiredKeyedService(typeof(ICertificadoCodafGeradorConteudo), It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task DadoLoteComCertificado_QuandoConverterLancaException_DeveAtualizarStatusErroENaoFazerUploadNemEnviarEmail()
        {
            _mockConfiguration.Setup(x => x["UrlFrontEnd"]).Returns(_faker.Internet.Url());

            var certificado = new DadosProcessamentoCertificadoCodafDto
            {
                Id = 99,
                CodigoCertificado = 555,
                HtmlContentSnapshot = "<p>{{NUM_SEQ}}</p>",
                NomeCompleto = "Error Test",
                EmailUsuario = "err@exemplo.com",
                TemRf = false,
                TipoParticipacao = TipoParticipacaoCodaf.Cursista,
                Emissor = "DRE-X"
            };

            _mockRepositorioCertificado.SetupSequence(r => r.ObterCertificadosParaProcessamentoAsync())
                .ReturnsAsync([certificado])
                .ReturnsAsync([]);

            _mockServicoRelatorio
                .Setup(s => s.ConveterHtmlCertificadoCodafParaPdfAsync(It.IsAny<HtmlCertificadoCodafDto>()))
                .ThrowsAsync(new Exception("Erro conversão"));

            var resultado = await _sut.Executar(new MensagemRabbit());

            await Task.Delay(50);

            resultado.Should().BeTrue();
            _mockRepositorioCertificado.Verify(r => r.AtualizarStatusProcessamentoAsync(certificado.Id,
                StatusProcessamentoCertificadoCodaf.ProcessadoComErro, null, It.Is<string>(m => m.Contains("Erro conversão"))), Times.Once);

            _mockServicoArmazenamento.Verify(a => a.UploadCertificadoCodafAsync(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Never);
            _mockMediator.Verify(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoLoteComCertificadoSemEmail_QuandoProcessar_NaoDeveEnviarNotificacaoPorEmail()
        {
            _mockConfiguration.Setup(x => x["UrlFrontEnd"]).Returns(_faker.Internet.Url());

            var certificado = new DadosProcessamentoCertificadoCodafDto
            {
                Id = 2,
                CodigoCertificado = 777,
                HtmlContentSnapshot = "<div>Sem email {{NUM_SEQ}}</div>",
                NomeCompleto = "Sem Email",
                EmailUsuario = string.Empty, 
                TemRf = true,
                TipoParticipacao = TipoParticipacaoCodaf.Cursista,
                Emissor = "DRE-Y"
            };

            _mockRepositorioCertificado.SetupSequence(r => r.ObterCertificadosParaProcessamentoAsync())
                .ReturnsAsync([certificado])
                .ReturnsAsync([]);

            _mockServicoRelatorio
                .Setup(s => s.ConveterHtmlCertificadoCodafParaPdfAsync(It.IsAny<HtmlCertificadoCodafDto>()))
                .ReturnsAsync([0x0]);

            _mockServicoArmazenamento
                .Setup(a => a.UploadCertificadoCodafAsync(It.IsAny<string>(), It.IsAny<byte[]>()))
                .ReturnsAsync("chave");

            var mockGerador = new Mock<ICertificadoCodafGeradorConteudo>();
            mockGerador.Setup(g => g.GerarConteudoEmail(It.IsAny<DadosProcessamentoCertificadoCodafDto>(), It.IsAny<string>()))
                .Returns(("T", "C"));
            _mockServiceProvider
                .Setup(sp => sp.GetRequiredKeyedService(typeof(ICertificadoCodafGeradorConteudo), It.IsAny<object>()))
                .Returns(mockGerador.Object);

            var resultado = await _sut.Executar(new MensagemRabbit());

            await Task.Delay(50);

            resultado.Should().BeTrue();
            _mockRepositorioCertificado.Verify(r => r.AtualizarStatusProcessamentoAsync(certificado.Id,
                StatusProcessamentoCertificadoCodaf.ProcessadoComSucesso, It.IsAny<string>(), null), Times.Once);

            _mockMediator.Verify(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}