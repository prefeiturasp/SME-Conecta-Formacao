using FluentAssertions;
using Moq;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafDeclaracoes;
using SME.ConectaFormacao.Infra.Dados.Estrategias.CodafDeclaracoes;
using SME.ConectaFormacao.Infra.Dados.Templates;

namespace SME.ConectaFormacao.Infra.Dados.Teste.Estrategias
{
    public class DeclaracaoCursistaSemRfEstrategiaTests
    {
        private readonly Mock<ITemplateService> _mockTemplateService;
        private readonly DeclaracaoCursistaSemRfStrategy _sut;

        public DeclaracaoCursistaSemRfEstrategiaTests()
        {
            _mockTemplateService = new Mock<ITemplateService>();
            _sut = new(_mockTemplateService.Object);
        }

        [Fact]
        public void GerarHtml_DeveGerarTextoCorreto_ParaCursistaSemRf()
        {
            // Arrange
            var dados = new DadosEmissaoDeclaracaoCodafDto
            {
                NomeCompleto = "João da Silva",
                Documento = "12345678910",
                NomeFormacao = "Curso .NET 8",
                DataRealizacao = new(2024, 01, 20, 0, 0, 0, DateTimeKind.Utc),
                HorasTotais = 20
            };

            _mockTemplateService.Setup(x => x.ObterTemplate("SME.ConectaFormacao.Infra.Dados.Templates.layout-declaracao-codaf.html"))
                .Returns("Base: {{TEXTO_DECLARACAO}} - Lateral: {{IMG_MOLDURA}} - {{HEADER}} - {{ASSINATURA}} - {{BRASAO}}");

            _mockTemplateService.Setup(x => x.ObterImagemBase64("SME.ConectaFormacao.Infra.Dados.Templates.Assets.header.png"))
                .Returns("img_header_base64");

            _mockTemplateService.Setup(x => x.ObterImagemBase64("SME.ConectaFormacao.Infra.Dados.Templates.Assets.brasao.png"))
                .Returns("img_brasao_base64");

            _mockTemplateService.Setup(x => x.ObterImagemBase64("SME.ConectaFormacao.Infra.Dados.Templates.Assets.assinatura.png"))
                .Returns("img_assinatura_base64");

            // Act
            var htmlFinal = _sut.GerarHtml(dados);

            htmlFinal.Should().Contain("Declaramos para os devidos fins que o(a) servidor(a), <b><i>João Da Silva</i></b>");
            htmlFinal.Should().Contain("CPF <b><i>123.456.789-10</i></b>");
            htmlFinal.Should().Contain("Curso .NET 8");
            htmlFinal.Should().Contain("participou do");

            htmlFinal.Should().Contain("img_header_base64");
          
            htmlFinal.Should().Contain("01/01/0001");
        }

        [Fact]
        public void GerarConteudoEmail_DeveRetornarTextoPersonalizado_ParaCursista()
        {
            // Arrange
            var dados = new DadosProcessamentoCodafDto
            {
                NomeCompleto = "Maria",
                NomeFormacao = "Curso Docker"
            };
            var url = "http://teste.com";

            // Act
            var (titulo, corpo) = _sut.GerarConteudoEmail(dados, url);

            // Assert
            titulo.Should().Contain("PARABÉNS! SUA DECLARAÇÃO FOI EMITIDA");
            titulo.Should().Contain("Curso Docker");

            corpo.Should().Contain("Olá <b>Maria</b>!");
            corpo.Should().Contain("participação como <b>cursista</b>"); // Valida que é texto de cursista
            corpo.Should().Contain(url);
        }
    }
}
