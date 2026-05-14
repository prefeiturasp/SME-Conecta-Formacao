using FluentAssertions;
using Moq;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
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
                DataInicio = new(2024, 01, 20),
                DataFim = new(2024, 01, 20),
                HorasTotais = 20,
                ConceitoFinal = "S",
                PercentualFrequencia = 100,
                TipoFormacao = "curso",
                DreCoordenadoria = "Secretaria Municipal",
                CodigoCertificado = 123,
                NumeroComunicado = 456,
                DataPublicacao = new(2024, 01, 20),
                PaginaDiarioOficial = 10,
                NumeroHomologacao = 789
            };

            var templateComPlaceholders = @"
                {{HEADER}}
                {{TEXTO_CERTIFICADO}}
                {{BRASAO}}
                {{SELO}}
                {{ASSINATURA}}
                {{COORDENADORIA_OU_DRE}}
                {{NUM_CODIGO_CERTIFICADO}}
                {{NUM_COMUNICADO}}
                {{DATA_PUBLICACAO_CODAF}}
                {{PAG_DIARIO_OFICIAL}}
                {{NUM_HOM_FORMACAO}}
                {{IMG_MOLDURA}}";

            _mockTemplateService.Setup(x => x.ObterTemplate("Templates/layout-certificado-codaf.html"))
                .Returns(templateComPlaceholders);

            _mockTemplateService.Setup(x => x.ObterImagemBase64("Templates/Assets/header.svg"))
                .Returns("img_cabecalho_base64");

            _mockTemplateService.Setup(x => x.ObterImagemBase64("Templates/Assets/brasao.png"))
                .Returns("img_brasao_base64");

            _mockTemplateService.Setup(x => x.ObterImagemBase64("Templates/Assets/selo.svg"))
                .Returns("img_selo_base64");

            _mockTemplateService.Setup(x => x.ObterImagemBase64("Templates/Assets/assinatura.png"))
                .Returns("img_assinatura_base64");

            // Act
            var htmlFinal = _sut.GerarHtml(dados);

            // Assert - Verifica o Texto Específico
            htmlFinal.Should().Contain("Certificamos para os devidos fins que o(a) servidor(a)");
            htmlFinal.Should().MatchRegex(@"João\s+Da\s+Silva"); // Aceita variações de espaço
            htmlFinal.Should().Contain("RF: 12345678910");
            htmlFinal.Should().Contain("Curso .NET 8");
            htmlFinal.Should().Contain("participou");

            // Assert - Verifica se a Imagem foi injetada
            htmlFinal.Should().Contain("img_cabecalho_base64");

            // Assert - Verifica formatação de datas e horas
            htmlFinal.Should().Contain("20/01/2024");
            htmlFinal.Should().Contain("20 horas");
            htmlFinal.Should().Contain("frequência de 100%");
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
            corpo.Should().Contain("participação como <b>cursista</b>");
            corpo.Should().Contain(url);
        }
    }
}
