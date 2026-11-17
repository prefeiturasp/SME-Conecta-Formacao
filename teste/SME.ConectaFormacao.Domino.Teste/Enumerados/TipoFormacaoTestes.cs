using FluentAssertions;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SME.ConectaFormacao.Domino.Teste.Enumerados
{
    public class TipoFormacaoTestes
    {
        [Theory]
        [InlineData(TipoFormacao.Curso, 1, "Curso")]
        [InlineData(TipoFormacao.Evento, 2, "Evento")]
        [InlineData(TipoFormacao.Conferencia, 3, "Conferência")]
        public void DadoTipoFormacao_QuandoVerificarValorENome_EntaoDeveCorresponderAoEsperado(
            TipoFormacao tipoFormacao,
            int valorEsperado,
            string nomeEsperado)
        {
            // Arrange & Act
            var membro = typeof(TipoFormacao).GetMember(tipoFormacao.ToString())[0];
            var atributo = membro.GetCustomAttribute<DisplayAttribute>();

            // Assert
            ((int)tipoFormacao).Should().Be(valorEsperado);
            tipoFormacao.Nome().Should().Be(nomeEsperado);
            atributo.Should().NotBeNull();
            atributo!.Name.Should().Be(nomeEsperado);
            Enum.IsDefined(typeof(TipoFormacao), tipoFormacao).Should().BeTrue();
        }
    }
}
