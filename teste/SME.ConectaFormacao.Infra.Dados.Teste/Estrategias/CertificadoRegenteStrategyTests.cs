using FluentAssertions;
using Moq;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Estrategias;
using SME.ConectaFormacao.Infra.Dados.Templates;

namespace SME.ConectaFormacao.Infra.Dados.Teste.Estrategias
{
    public class CertificadoRegenteStrategyTests
    {
        private readonly Mock<ITemplateService> _mockTemplateService;
        private readonly CertificadoRegenteStrategy _sut;

        public CertificadoRegenteStrategyTests()
        {
            _mockTemplateService = new Mock<ITemplateService>();
            _sut = new CertificadoRegenteStrategy(_mockTemplateService.Object);
        }

        [Fact]
        public void GerarHtml_DeveGerarTextoCorreto_ParaRegente()
        {
            // Arrange
            var dados = new DadosEmissaoCertificadoCodafDto
            {
                NomeCompleto = "João da Silva",
                Documento = "1234567",
                NomeFormacao = "Curso .NET 8",
                DataInicio = new(2024, 01, 20, 0, 0, 0, DateTimeKind.Utc),
                DataFim = new(2024, 01, 20, 0, 0, 0, DateTimeKind.Utc),
                HorasTotais = 20,
                ConceitoFinal = "S",
                PercentualFrequencia = 100,
                TipoFormacao = "o",
                Emissor = ""
            };

            _mockTemplateService.Setup(x => x.ObterTemplate(It.IsAny<string>()))
                .Returns("Base: {{TEXTO_CERTIFICADO}} - Lateral: {{IMG_MOLDURA}}");

            _mockTemplateService.Setup(x => x.ObterImagemBase64(It.IsAny<string>()))
                .Returns("img_comum");

            _mockTemplateService.Setup(x => x.ObterImagemBase64("header.png"))
                .Returns("img_header_base64");

            // Act
            var htmlFinal = _sut.GerarHtml(dados);

            // Assert - Verifica o Texto Específico
            htmlFinal.Should().Contain("Certificamos para os devidos fins que o(a) servidor(a)");
            htmlFinal.Should().Contain("<i>João Da Silva</i>");
            htmlFinal.Should().Contain("RF: <b><i>123.456.7</b></i>");
            htmlFinal.Should().Contain("Curso .NET 8");
            htmlFinal.Should().Contain("ministrou o");
            htmlFinal.Should().Contain("20 horas");
        }

        [Fact]
        public void GerarConteudoEmail_DeveRetornarTextoPersonalizado_ParaRegente()
        {
            // Arrange
            var dados = new DadosProcessamentoCertificadoCodafDto
            {
                NomeCompleto = "Maria",
                NomeFormacao = "Curso Docker"
            };
            var url = "http://teste.com";

            // Act
            var (titulo, corpo) = _sut.GerarConteudoEmail(dados, url);

            // Assert
            titulo.Should().Contain("PARABÉNS! SEU CERTIFICADO FOI EMITIDO");
            titulo.Should().Contain("Curso Docker");

            corpo.Should().Contain("Olá <b>Maria</b>!");
            corpo.Should().Contain("participação como <b>regente</b>");
            corpo.Should().Contain(url);
        }
    }
}
