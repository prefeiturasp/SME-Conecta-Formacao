using Bogus;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using Moq;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Estrategias;
using SME.ConectaFormacao.Infra.Dados.Templates;

namespace SME.ConectaFormacao.Infra.Dados.Teste.Estrategias
{
    public class CertificadoRegenteSemRfStrategyTests
    {
        private readonly Mock<ITemplateService> _mockTemplateService;
        private readonly CertificadoRegenteSemRfStrategy _strategy;
        private readonly Faker _faker;

        public CertificadoRegenteSemRfStrategyTests()
        {
            _mockTemplateService = new Mock<ITemplateService>();
            _strategy = new(_mockTemplateService.Object);
            _faker = new();
        }

        [Fact]
        public void GerarHtml_DeveGerarTextoCorreto_ParaRegenteSemRf()
        {
            // Arrange
            var dados = new DadosEmissaoCertificadoCodafDto
            {
                NomeCompleto = _faker.Person.FullName,
                Documento = _faker.Person.Cpf(),
                NomeFormacao = "Curso .NET 8",
                DataInicio = new(2024, 01, 20, 0, 0, 0, DateTimeKind.Utc),
                DataFim = new(2024, 01, 20, 0, 0, 0, DateTimeKind.Utc),
                HorasTotais = 99,
                CargaHorariaTotalOutra = "30:00",
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
            var htmlFinal = _strategy.GerarHtml(dados);
            // Assert
            htmlFinal.Should().Contain("Certificamos para os devidos fins que o(a) servidor(a)");
            htmlFinal.Should().Contain($"<i>{dados.NomeCompleto}</i>");
            htmlFinal.Should().NotContain("RF:");
            htmlFinal.Should().Contain($"CPF: <b><i>{dados.Documento}</b></i>");
            htmlFinal.Should().Contain(dados.NomeFormacao);
            htmlFinal.Should().Contain("ministrou o");
            htmlFinal.Should().Contain("30 horas");
        }

        [Fact]
        public void GerarConteudoEmail_DeveRetornarTextoPersonalizado_ParaRegente()
        {
            // Arrange
            var dados = new DadosProcessamentoCertificadoCodafDto
            {
                NomeCompleto = _faker.Person.FullName,
                NomeFormacao = "Curso .NET 8",
                Emissor = ""
            };
            var urlAcesso = "https://conecta.educacao.sp.gov.br/certificados";
            // Act
            var (titulo, corpo) = _strategy.GerarConteudoEmail(dados, urlAcesso);
            // Assert
            titulo.Should().Contain("PARABÉNS! SEU CERTIFICADO FOI EMITIDO");
            titulo.Should().Contain(dados.NomeFormacao);
            corpo.Should().Contain($"Olá <b>{dados.NomeCompleto}</b>! Parabéns!");
            corpo.Should().Contain($"Você concluiu sua participação como <b>regente</b> na formação <b>{dados.NomeFormacao}</b>.");
            corpo.Should().Contain($"clicando <a href='{urlAcesso}' target='_blank'>aqui</a>");
        }
    }
}
