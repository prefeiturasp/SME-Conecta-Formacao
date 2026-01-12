using FluentAssertions;
using SME.ConectaFormacao.Infra.Servicos.Utilitarios;

namespace SME.ConectaFormacao.Infra.Servicos.Teste.Utilitarios
{
    public class UtilValidacoesTests
    {
        [Theory]
        [InlineData("111.444.777-35")] // CPF Válido (Formato com máscara)
        [InlineData("11144477735")]    // CPF Válido (Sem máscara)
        public void DadoCpfValido_QuandoValidar_EntaoDeveRetornarVerdadeiro(string cpf)
        {
            // Arrange & Act
            var resultado = UtilValidacoes.CpfEhValido(cpf);

            // Assert
            resultado.Should().BeTrue();
        }

        [Theory]
        [InlineData("111.111.111-11")] // Dígitos iguais
        [InlineData("123.456.789-00")] // Dígito verificador errado
        [InlineData("12345")]          // Tamanho inválido
        [InlineData("")]               // Vazio
        public void DadoCpfInvalido_QuandoValidar_EntaoDeveRetornarFalso(string cpf)
        {
            // Arrange & Act
            var resultado = UtilValidacoes.CpfEhValido(cpf);

            // Assert
            resultado.Should().BeFalse();
        }
    }
}
