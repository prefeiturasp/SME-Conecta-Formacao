using Bogus;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using Moq;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Estrategias.Base;
using SME.ConectaFormacao.Infra.Dados.Templates;

namespace SME.ConectaFormacao.Infra.Dados.Teste.Estrategias
{
    public class CertificadoEstrategiaBaseTests
    {
        private readonly Mock<ITemplateService> _mockTemplateService;
        private readonly Faker _faker;
        private readonly TestableCertificadoEstrategiaBase _sut;

        public CertificadoEstrategiaBaseTests()
        {
            _mockTemplateService = new Mock<ITemplateService>();
            _faker = new();

            // Setup Padrão dos Mocks para a Base
            _mockTemplateService.Setup(x => x.ObterTemplateCertificado(It.IsAny<string>()))
                .Returns("<html>{{IMG_BRASAO_TITULO_SME}} - {{ANO_ATUAL}}</html>"); // Template simplificado

            _mockTemplateService.Setup(x => x.ObterImagemBase64(It.IsAny<string>()))
                .Returns("base64_fake");

            _sut = new TestableCertificadoEstrategiaBase(_mockTemplateService.Object);
        }

        [Fact]
        public void ObterLayoutBase_DeveSubstituirTokensComuns()
        {
            // Arrange
            var dados = new DadosEmissaoCertificadoCodafDto
            {
                NumeroComunicado = 123,
                DataPublicacao = new(2023, 10, 10, 0, 0, 0, DateTimeKind.Utc),
                NumeroHomologacao = 999,
                NomeCompleto = _faker.Person.FullName,
                Documento = _faker.Person.Cpf(),
                NomeFormacao = _faker.Lorem.Sentence(3),
            };

            // Act
            var resultado = _sut.ExporObterLayoutBase(dados);

            // Assert
            resultado.Should().Contain("base64_fake");
            resultado.Should().Contain(DateTime.Now.Year.ToString());

            _mockTemplateService.Verify(x => x.ObterTemplateCertificado("layout-certificado-codaf.html"), Times.Once);
        }

        private class TestableCertificadoEstrategiaBase(ITemplateService templateService) : CertificadoEstrategiaBase(templateService)
        {
            public string ExporObterLayoutBase(DadosEmissaoCertificadoCodafDto dados) => ObterLayoutBase(dados);
        }
    }
}
