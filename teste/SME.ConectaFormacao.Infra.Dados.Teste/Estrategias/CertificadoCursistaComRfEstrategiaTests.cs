using FluentAssertions;
using Moq;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;
using SME.ConectaFormacao.Infra.Dados.Estrategias;
using SME.ConectaFormacao.Infra.Dados.Templates;

namespace SME.ConectaFormacao.Infra.Dados.Teste.Estrategias
{
    public class CertificadoCursistaComRfEstrategiaTests
    {
        private readonly Mock<ITemplateService> _mockTemplateService;
        private readonly CertificadoCursistaComRfStrategy _sut;

        public CertificadoCursistaComRfEstrategiaTests()
        {
            _mockTemplateService = new Mock<ITemplateService>();
            _sut = new CertificadoCursistaComRfStrategy(_mockTemplateService.Object);
        }

        [Fact]
        public void GerarHtml_DeveGerarTextoCorreto_ParaCursistaComRf()
        {
            // Arrange
            var dados = new DadosEmissaoCertificadoCodafDto
            {
                NomeCompleto = "João da Silva",
                Documento = "1234567",
                NomeFormacao = "Curso .NET 8",
                DataRealizacao = new(2024, 01, 20, 0, 0, 0, DateTimeKind.Utc),
                HorasTotais = 20,
                ConceitoFinal = "S",
                PercentualFrequencia = 100
            };

            _mockTemplateService.Setup(x => x.ObterTemplateCertificado(It.IsAny<string>()))
                .Returns("Base: {{TEXTO_CERTIFICADO}} - Lateral: {{IMG_MOLDURA_LATERAL}}");

            _mockTemplateService.Setup(x => x.ObterImagemBase64("barra_lateral_padrao_certificado_codaf.png"))
                .Returns("img_lateral_base64");

            _mockTemplateService.Setup(x => x.ObterImagemBase64(It.Is<string>(s => s != "barra_lateral_padrao_certificado_codaf.png")))
                .Returns("img_comum");

            // Act
            var htmlFinal = _sut.GerarHtml(dados);

            // Assert - Verifica o Texto Específico
            htmlFinal.Should().Contain("Certificamos para os devidos fins que o(a) servidor(a), <b>João da Silva</b>");
            htmlFinal.Should().Contain("R.F. 1234567");
            htmlFinal.Should().Contain("Curso .NET 8");
            htmlFinal.Should().Contain("participou do Evento");

            // Assert - Verifica se a Imagem Lateral Especifica foi injetada
            htmlFinal.Should().Contain("img_lateral_base64");

            // Assert - Verifica formatação de datas e horas
            htmlFinal.Should().Contain("20/01/2024");
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
