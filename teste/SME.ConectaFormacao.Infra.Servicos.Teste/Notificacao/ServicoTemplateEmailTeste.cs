using Bogus;
using FluentAssertions;
using SME.ConectaFormacao.Infra.Servicos.Notificacao;

namespace SME.ConectaFormacao.Infra.Servicos.Teste.Notificacao
{
    public class ServicoTemplateEmailTeste
    {
        private readonly ServicoTemplateEmail _servicoTemplateEmail;
        private readonly Faker _faker;

        public ServicoTemplateEmailTeste()
        {
            _servicoTemplateEmail = new();
            _faker = new();
        }

        [Fact]
        public async Task DadoDadosValidos_QuandoObterHtmlEmEspera_EntaoSubstituiPlaceholdersCorretamente()
        {
            // Arrange
            var nomeCursista = _faker.Name.FullName();
            var nomeFormacao = _faker.Lorem.Word();

            // Act
            var htmlGerado = await _servicoTemplateEmail.ObterHtmlInscricaoEmEsperaAsync(nomeCursista, nomeFormacao);

            // Assert
            htmlGerado.Should().Contain(nomeCursista);
            htmlGerado.Should().Contain(nomeFormacao);
            htmlGerado.Should().NotContain("{{NOME_CURSISTA}}");
            htmlGerado.Should().NotContain("{{NOME_FORMACAO}}");
            htmlGerado.Should().Contain("Inscrição em Lista de Espera");
        }
    }
}
