using FluentAssertions;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Dominio.Utilitarios;
using System;
using Xunit;

namespace SME.ConectaFormacao.Domino.Teste.Utilitarios
{
    public class GeradorChaveIdempotenciaTestes
    {
        [Fact]
        public void DadoParametrosComJanelaTemporal_QuandoGerar_EntaoDeveRetornarChaveUnica()
        {
            // Arrange
            var email = " teste@gmail.com ";
            var titulo = " Titulo do email ";
            var correlacaoId = Guid.NewGuid();
            var janela = new DateTime(2023, 10, 10, 15, 30, 0);

            var emailEsperado = email.Trim().ToLowerInvariant();
            var tituloEsperado = titulo.Trim();
            var janelaEsperada = janela.ToString("yyyyMMddHH");
            var chaveBaseEsperada = $"{correlacaoId}-{emailEsperado}-{tituloEsperado}-{janelaEsperada}";
            var hashEsperado = chaveBaseEsperada.GerarHashSHA256();

            // Act
            var resultado = GeradorChaveIdempotencia.Gerar(email, titulo, correlacaoId, janela, true);

            // Assert
            resultado.Should().Be(hashEsperado);
        }

        [Fact]
        public void DadoParametrosSemJanelaTemporal_QuandoGerar_EntaoDeveRetornarChaveSemJanela()
        {
            // Arrange
            var email = "teste@gmail.com";
            var titulo = "Titulo";
            var correlacaoId = Guid.NewGuid();

            var chaveBaseEsperada = $"{correlacaoId}-{email}-{titulo}";
            var hashEsperado = chaveBaseEsperada.GerarHashSHA256();

            // Act
            var resultado = GeradorChaveIdempotencia.Gerar(email, titulo, correlacaoId, null, false);

            // Assert
            resultado.Should().Be(hashEsperado);
        }

        [Fact]
        public void DadoParametrosNulosCorrelacao_QuandoGerar_EntaoDeveUsarDefaultSemCorrelacao()
        {
            // Arrange
            var email = "teste@gmail.com";
            var titulo = "Titulo";
            
            var chaveBaseEsperada = $"sem-correlacao-{email}-{titulo}";
            var hashEsperado = chaveBaseEsperada.GerarHashSHA256();

            // Act
            var resultado = GeradorChaveIdempotencia.Gerar(email, titulo, null, null, false);

            // Assert
            resultado.Should().Be(hashEsperado);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void DadoEmailInvalido_QuandoGerar_EntaoDeveLancarArgumentException(string emailInvalido)
        {
            // Arrange
            var titulo = "Titulo Valido";

            // Act
            var acao = () => GeradorChaveIdempotencia.Gerar(emailInvalido, titulo);

            // Assert
            acao.Should().Throw<ArgumentException>()
                .WithMessage("*Email do destinatário é obrigatório*");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void DadoTituloInvalido_QuandoGerar_EntaoDeveLancarArgumentException(string tituloInvalido)
        {
            // Arrange
            var email = "teste@gmail.com";

            // Act
            var acao = () => GeradorChaveIdempotencia.Gerar(email, tituloInvalido);

            // Assert
            acao.Should().Throw<ArgumentException>()
                .WithMessage("*Título do e-mail é obrigatório*");
        }

        [Fact]
        public void DadoParametrosValidos_QuandoGerarParaNotificacao_EntaoDeveRetornarChaveUnica()
        {
            // Arrange
            var notificacaoId = 1L;
            var notificacaoUsuarioId = 2L;
            var email = " teste@gmail.com ";
            var titulo = " Titulo do email ";

            var emailEsperado = email.Trim().ToLowerInvariant();
            var tituloEsperado = titulo.Trim();
            
            var chaveBaseEsperada = $"{notificacaoId}-{notificacaoUsuarioId}-{emailEsperado}-{tituloEsperado}";
            var hashEsperado = chaveBaseEsperada.GerarHashSHA256();

            // Act
            var resultado = GeradorChaveIdempotencia.GerarParaNotificacao(notificacaoId, notificacaoUsuarioId, email, titulo);

            // Assert
            resultado.Should().Be(hashEsperado);
        }
    }
}
