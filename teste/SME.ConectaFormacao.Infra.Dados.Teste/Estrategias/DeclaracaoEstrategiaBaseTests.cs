using Bogus;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using Moq;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafDeclaracoes;
using SME.ConectaFormacao.Infra.Dados.Estrategias.Base;
using SME.ConectaFormacao.Infra.Dados.Templates;

namespace SME.ConectaFormacao.Infra.Dados.Teste.Estrategias
{
    public class DeclaracaoEstrategiaBaseTests
    {
        private readonly Mock<ITemplateService> _mockTemplateService;
        private readonly Faker _faker;
        private readonly TestableDeclaracaoEstrategiaBase _sut;

        public DeclaracaoEstrategiaBaseTests()
        {
            _mockTemplateService = new Mock<ITemplateService>();
            _faker = new();

            // Setup Padrão dos Mocks para a Base
            _mockTemplateService.Setup(x => x.ObterTemplate("SME.ConectaFormacao.Infra.Dados.Templates.layout-declaracao-codaf.html"))
                .Returns("<html>{{HEADER}} - {{BRASAO}} - {{ASSINATURA}} - {{EMISSOR}} - {{NUM_CODIGO_DECLARACAO}} - {{DATA_PUBLICACAO_CODAF}} - {{NUM_HOM_FORMACAO}}</html>");

            _mockTemplateService.Setup(x => x.ObterImagemBase64("SME.ConectaFormacao.Infra.Dados.Templates.Assets.header.png"))
                .Returns("base64_fake");

            _mockTemplateService.Setup(x => x.ObterImagemBase64("SME.ConectaFormacao.Infra.Dados.Templates.Assets.brasao.png"))
                .Returns("base64_fake");

            _mockTemplateService.Setup(x => x.ObterImagemBase64("SME.ConectaFormacao.Infra.Dados.Templates.Assets.assinatura.png"))
                .Returns("base64_fake");

            _sut = new TestableDeclaracaoEstrategiaBase(_mockTemplateService.Object);
        }

        [Fact]
        public void ObterLayoutBase_DeveSubstituirTokensComuns()
        {
            // Arrange
            var dados = new DadosEmissaoDeclaracaoCodafDto
            {
                DataPublicacao = new(2023, 10, 10, 0, 0, 0, DateTimeKind.Utc),
                NumeroHomologacao = 999,
                Emissor = "DRE Teste",
                NomeCompleto = _faker.Person.FullName,
                Documento = _faker.Person.Cpf(),
                NomeFormacao = _faker.Lorem.Sentence(3),
            };

            // Act
            var resultado = _sut.ExporObterLayoutBase(dados);

            // Assert
            resultado.Should().Contain("base64_fake");

            _mockTemplateService.Verify(x => x.ObterTemplate("SME.ConectaFormacao.Infra.Dados.Templates.layout-declaracao-codaf.html"), Times.Once);
            _mockTemplateService.Verify(x => x.ObterImagemBase64("SME.ConectaFormacao.Infra.Dados.Templates.Assets.header.png"), Times.Once);
            _mockTemplateService.Verify(x => x.ObterImagemBase64("SME.ConectaFormacao.Infra.Dados.Templates.Assets.brasao.png"), Times.Once);
            _mockTemplateService.Verify(x => x.ObterImagemBase64("SME.ConectaFormacao.Infra.Dados.Templates.Assets.selo.svg"), Times.Once);
            _mockTemplateService.Verify(x => x.ObterImagemBase64("SME.ConectaFormacao.Infra.Dados.Templates.Assets.assinatura.png"), Times.Once);
        }

        private class TestableDeclaracaoEstrategiaBase(ITemplateService templateService) : DeclaracaoEstrategiaBase(templateService)
        {
            public string ExporObterLayoutBase(DadosEmissaoDeclaracaoCodafDto dados) => ObterLayoutBase(dados);
        }
    }
}
