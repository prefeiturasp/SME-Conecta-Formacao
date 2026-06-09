using FluentAssertions;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using System.Reflection;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterModeloTermoResponsabilidadeCodafTestes
    {
        [Fact]
        public void DadoSolicitacaoModelo_QuandoRecursoExistir_DeveRetornarArquivoDto()
        {
            // Arrange
            var casoDeUso = new CasoDeUsoObterModeloTermoResponsabilidadeCodaf();

            // Act
            var resultado = casoDeUso.Executar();

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.Nome.Should().Be("TermoResponsabilidadeModelo.pdf");
            resultado.Dados.ContentType.Should().Be("application/pdf");
            resultado.Dados.Stream.Should().NotBeNull();
        }

        [Fact]
        public void DadoSolicitacaoModelo_QuandoRecursoNaoExistir_DeveRetornarErroNaoEncontrado()
        {
            // Arrange
            var assemblyMock = new Mock<Assembly>();
            assemblyMock
                .Setup(a => a.GetManifestResourceStream(It.IsAny<string>()))
                .Returns((Stream?)null);

            var casoDeUso = new CasoDeUsoObterModeloTermoResponsabilidadeCodaf(assemblyMock.Object);

            // Act
            var resultado = casoDeUso.Executar();

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Dados.Should().BeNull();
            resultado.MensagensErro.Should().Contain(m => m.Contains("Não foi possível localizar o modelo do termo de responsabilidade."));
        }
    }
}
