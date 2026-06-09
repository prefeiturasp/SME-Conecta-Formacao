using FluentAssertions;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Domino.Teste.Comum
{
    public class ResolvedorDocumentoUsuarioTestes
    {
        [Fact]
        public void DadoUmLoginDoTipoRF_QuandoResolverDocumento_EntaoDeveRetornarDocumentoDoTipoRF()
        {
            // Arrange
            var login = "0123456";
            var cpf = "12345678901";

            // Act
            var (valor, tipo) = ResolvedorDocumentoUsuario.Resolver(login, cpf);

            // Assert
            valor.Should().Be("0123456");
            tipo.Should().Be(TipoDocumentoUsuario.Rf);
        }

        [Theory]
        [InlineData("A123456")]
        [InlineData("RA3996826")]
        [InlineData("12345678901")]
        public void DadoUmLoginQueNaoEhRF_QuandoResolverDocumento_EntaoDeveRetornarDocumentoDoTipoCPF(string login)
        {
            // Arrange
            var cpf = "12345678901";

            // Act
            var (valor, tipo) = ResolvedorDocumentoUsuario.Resolver(login, cpf);

            // Assert
            valor.Should().Be("12345678901");
            tipo.Should().Be(TipoDocumentoUsuario.Cpf);
        }

        [Fact]
        public void DadoUmValorVazio_QuandoFormatarValor_EntaoDeveRetornarValorVazio()
        {
            // Arrange
            var valor = "";
            var tipo = TipoDocumentoUsuario.Cpf;

            // Act
            var resultado = ResolvedorDocumentoUsuario.FormatarValor(valor, tipo);

            // Assert
            resultado.Should().Be("");
        }

        [Fact]
        public void DadoUmValorDoTipoCPF_QuandoFormatarValor_EntaoDeveRetornarValorFormatadoComoCPF()
        {
            // Arrange
            var valor = "12345678901";
            var tipo = TipoDocumentoUsuario.Cpf;

            // Act
            var resultado = ResolvedorDocumentoUsuario.FormatarValor(valor, tipo);

            // Assert
            resultado.Should().Be("123.456.789-01");
        }

        [Fact]
        public void DadoUmValorDoTipoRF_QuandoFormatarValor_EntaoDeveRetornarValorFormatadoComoRF()
        {
            // Arrange
            var valor = "0123456";
            var tipo = TipoDocumentoUsuario.Rf;

            // Act
            var resultado = ResolvedorDocumentoUsuario.FormatarValor(valor, tipo);

            // Assert
            resultado.Should().Be("012.345.6");
        }
    }
}