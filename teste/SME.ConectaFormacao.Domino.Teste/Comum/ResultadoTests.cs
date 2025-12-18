using FluentAssertions;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Domino.Teste.Comum
{
    public class ResultadoTests
    {
        [Fact]
        public void DadoUmDadoValido_QuandoCriarResultadoDeSucesso_EntaoDeveRetornarObjetoComSucessoVerdadeiro()
        {
            // Arrange
            var dadosEsperados = "Teste de Dados";

            // Act
            var resultado = Resultado<string>.DeSucesso(dadosEsperados);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().Be(dadosEsperados);
            resultado.TipoFalha.Should().Be(TipoFalha.Nenhuma);
            resultado.MensagensErro.Should().BeEmpty();
        }

        [Fact]
        public void DadoUmaMensagemDeErro_QuandoCriarResultadoDeFalha_EntaoDeveRetornarObjetoComSucessoFalso()
        {
            // Arrange
            var mensagemErro = "Erro de validação";
            var tipoFalha = TipoFalha.Validacao;

            // Act
            var resultado = Resultado<string>.DeFalha(tipoFalha, mensagemErro);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Dados.Should().BeNull();
            resultado.TipoFalha.Should().Be(tipoFalha);
            resultado.MensagensErro.Should().HaveCount(1);
            resultado.MensagensErro.Should().Contain(mensagemErro);
        }

        [Fact]
        public void DadoListaDeErros_QuandoCriarResultadoDeFalha_EntaoDeveConterTodasAsMensagens()
        {
            // Arrange
            var mensagens = new List<string> { "Erro 1", "Erro 2" };
            var tipoFalha = TipoFalha.RegraDeNegocio;

            // Act
            var resultado = Resultado<int>.DeFalha(tipoFalha, mensagens);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(tipoFalha);
            resultado.MensagensErro.Should().HaveCount(2);
            resultado.MensagensErro.Should().BeEquivalentTo(mensagens);
        }

        [Fact]
        public void DadoUmObjetoT_QuandoUsarOperadorImplicito_EntaoDeveConverterParaResultadoDeSucesso()
        {
            // Arrange
            int valor = 123;

            // Act
            Resultado<int> resultado = valor;

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().Be(valor);
            resultado.TipoFalha.Should().Be(TipoFalha.Nenhuma);
        }

        [Fact]
        public void DadoUmStructErro_QuandoUsarOperadorImplicitoParaGenerico_EntaoDeveConverterParaResultadoDeFalha()
        {
            // Arrange
            var erro = Erro.NaoEncontrado("Aluno não encontrado");

            // Act
            Resultado<ClasseFicticiaParaTesteDto> resultado = erro;

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Dados.Should().BeNull();
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);
            resultado.MensagensErro.Should().HaveCount(1);
            resultado.MensagensErro.Should().Contain("Aluno não encontrado");
        }

        [Fact]
        public void DadoUmaChamadaSemArgumentos_QuandoCriarResultadoDeSucessoVazio_EntaoDeveTerSucessoVerdadeiro()
        {
            // Arrange & Act
            var resultado = Resultado.DeSucesso();

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().BeTrue();
            resultado.TipoFalha.Should().Be(TipoFalha.Nenhuma);
            resultado.MensagensErro.Should().BeEmpty();
        }

        [Fact]
        public void DadoUmaMensagemDeErro_QuandoCriarResultadoDeFalhaVazio_EntaoDeveTerSucessoFalso()
        {
            // Arrange
            var mensagem = "Falha interna";
            var tipo = TipoFalha.ErroInterno;

            // Act
            var resultado = Resultado.DeFalha(tipo, mensagem);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(tipo);
            resultado.MensagensErro.Should().HaveCount(1);
            resultado.MensagensErro.Should().Contain(mensagem);
        }

        [Fact]
        public void DadoUmStructErro_QuandoUsarOperadorImplicitoParaVazio_EntaoDeveConverterParaResultadoDeFalha()
        {
            // Arrange
            var erro = Erro.Negocio("Operação inválida no período");

            // Act
            Resultado resultado = erro;

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.MensagensErro.Should().Contain("Operação inválida no período");
        }
    }

    internal class ClasseFicticiaParaTesteDto { }
}
