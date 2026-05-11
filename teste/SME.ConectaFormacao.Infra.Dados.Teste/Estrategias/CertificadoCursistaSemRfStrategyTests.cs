using FluentAssertions;
using Moq;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Estrategias;
using SME.ConectaFormacao.Infra.Dados.Templates;

namespace SME.ConectaFormacao.Infra.Dados.Teste.Estrategias
{
    public class CertificadoCursistaSemRfEstrategiaTests
    {
        private readonly Mock<ITemplateService> _mockTemplateService;
        private readonly CertificadoCursistaSemRfStrategy _sut;

        public CertificadoCursistaSemRfEstrategiaTests()
        {
            _mockTemplateService = new Mock<ITemplateService>();
            _sut = new(_mockTemplateService.Object);
        }

        [Fact]
        public void GerarHtml_DeveGerarTextoCorreto_ParaCursistaSemRf()
        {
            // Arrange
            var dados = new DadosEmissaoCertificadoCodafDto
            {
                NomeCompleto = "João da Silva",
                Documento = "12345678910",
                NomeFormacao = "Curso .NET 8",
                DataRealizacao = new(2024, 01, 20, 0, 0, 0, DateTimeKind.Utc),
                HorasTotais = 20,
                ConceitoFinal = "S",
                PercentualFrequencia = 100
            };

            _mockTemplateService.Setup(x => x.ObterTemplate(It.IsAny<string>()))
                .Returns("Base: {{TEXTO_CERTIFICADO}} - Lateral: {{IMG_MOLDURA}}");

            _mockTemplateService.Setup(x => x.ObterImagemBase64(It.IsAny<string>()))
                .Returns("img_comum");

            _mockTemplateService.Setup(x => x.ObterImagemBase64("header.jpg"))
                .Returns("img_header_base64");

            _mockTemplateService.Setup(x => x.ObterImagemBase64("assinatura_secretario.png"))
                .Returns("img_assinatura_base64");

            _mockTemplateService.Setup(x => x.ObterImagemBase64("brasao_prefeitura.png"))
                .Returns("img_brasao_base64");

            // Act
            var htmlFinal = _sut.GerarHtml(dados);

            htmlFinal.Should().Contain("Certificamos para os devidos fins que o(a) servidor(a), <b>João Da Silva</b>");
            htmlFinal.Should().Contain("CPF 123.456.789-10");
            htmlFinal.Should().Contain("Curso .NET 8");
            htmlFinal.Should().Contain("participou do");

            htmlFinal.Should().Contain("img_header_base64");
          
            htmlFinal.Should().Contain("01/01/0001");
        }

        [Fact]
        public void GerarConteudoEmail_DeveRetornarTextoPersonalizado_ParaCursista()
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
            corpo.Should().Contain("participação como <b>cursista</b>"); // Valida que é texto de cursista
            corpo.Should().Contain(url);
        }
    }
}
