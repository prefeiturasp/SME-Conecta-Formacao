using FluentAssertions;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Xunit;

namespace SME.ConectaFormacao.Domino.Teste.Enumerados
{
    public class TipoFormacaoTestes
    {
        [Fact]
        public void DadoTipoFormacaoConferencia_QuandoObterValorInteiro_EntaoDeveSerIgualATres()
        {
            // Arrange & Act
            var valor = (int)TipoFormacao.Conferencia;

            // Assert
            valor.Should().Be(3);
        }

        [Fact]
        public void DadoValorTres_QuandoConverterParaTipoFormacao_EntaoDeveRetornarConferencia()
        {
            // Arrange & Act
            var tipoFormacao = (TipoFormacao)3;

            // Assert
            tipoFormacao.Should().Be(TipoFormacao.Conferencia);
        }

        [Fact]
        public void DadoTipoFormacaoConferencia_QuandoObterNome_EntaoDeveRetornarConferenciaComAcento()
        {
            // Arrange & Act
            var nome = TipoFormacao.Conferencia.Nome();

            // Assert
            nome.Should().Be("Conferência");
        }

        [Fact]
        public void DadoTipoFormacaoConferencia_QuandoObterDisplayAttribute_EntaoDeveEstarConfiguradoCorretamente()
        {
            // Arrange
            var membro = typeof(TipoFormacao).GetMember(nameof(TipoFormacao.Conferencia))[0];

            // Act
            var atributo = membro.GetCustomAttribute<DisplayAttribute>();

            // Assert
            atributo.Should().NotBeNull();
            atributo!.Name.Should().Be("Conferência");
        }

        [Theory]
        [InlineData(TipoFormacao.Curso, 1, "Curso")]
        [InlineData(TipoFormacao.Evento, 2, "Evento")]
        [InlineData(TipoFormacao.Conferencia, 3, "Conferência")]
        public void DadoTipoFormacao_QuandoVerificarValorENome_EntaoDeveCorresponderAoEsperado(
            TipoFormacao tipoFormacao,
            int valorEsperado,
            string nomeEsperado)
        {
            // Assert
            ((int)tipoFormacao).Should().Be(valorEsperado);
            tipoFormacao.Nome().Should().Be(nomeEsperado);
            Enum.IsDefined(typeof(TipoFormacao), tipoFormacao).Should().BeTrue();
        }

        [Theory]
        [InlineData("Curso", TipoFormacao.Curso)]
        [InlineData("Evento", TipoFormacao.Evento)]
        [InlineData("Conferencia", TipoFormacao.Conferencia)]
        public void DadoNomeEnum_QuandoFazerParse_EntaoDeveRetornarTipoFormacaoCorreto(
            string nomeEnum,
            TipoFormacao esperado)
        {
            // Act
            var resultado = Enum.Parse<TipoFormacao>(nomeEnum);

            // Assert
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(4)]
        [InlineData(-1)]
        [InlineData(99)]
        public void DadoValorInteiroInvalido_QuandoVerificarSeEstaDefinido_EntaoDeveRetornarFalso(int valorInvalido)
        {
            // Act
            var definido = Enum.IsDefined(typeof(TipoFormacao), valorInvalido);

            // Assert
            definido.Should().BeFalse();
        }
    }
}
