using Bogus;
using FluentAssertions;
using Moq.AutoMock;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;
using System.Reflection;

namespace SME.ConectaFormacao.Domino.Teste.Entidades
{
    public class CodafSuplementarTestes
    {
        private readonly AutoMocker mocker;

        public CodafSuplementarTestes()
        {
            mocker = new AutoMocker();
        }

        [Fact]
        public void DadoIdValido_QuandoInstanciar_EntaoDeveDefinirIdEStatusIniciado()
        {
            // Arrange
            var id = new Faker().Random.Long(1, 1000);

            // Act
            var sut = new CodafSuplementar(id);

            // Assert
            sut.CodafId.Should().Be(id);
            sut.Status.Should().Be(StatusCodafSuplementar.Iniciado);
        }

        [Fact]
        public void DadoIdEDadosPublicacao_QuandoInstanciar_EntaoDeveDefinirPropriedades()
        {
            // Arrange
            var id = new Faker().Random.Long(1, 1000);
            var dadosPublicacao = GerarDadosPublicacaoLista();

            // Act
            var sut = new CodafSuplementar(id, dadosPublicacao);

            // Assert
            sut.CodafId.Should().Be(id);
            sut.Status.Should().Be(StatusCodafSuplementar.Iniciado);
            sut.DataPublicacao.Should().Be(dadosPublicacao.DataPublicacao);
            sut.DataPublicacaoDom.Should().Be(dadosPublicacao.DataPublicacaoDom);
            sut.NumeroComunicado.Should().Be(dadosPublicacao.NumeroComunicado);
            sut.PaginaComunicadoDom.Should().Be(dadosPublicacao.PaginaComunicadoDom);
            sut.CodigoCursoEol.Should().Be(dadosPublicacao.CodigoCursoEol);
            sut.CodigoNivel.Should().Be(dadosPublicacao.CodigoNivel);
            sut.Observacao.Should().Be(dadosPublicacao.Observacao);
        }

        [Fact]
        public void DadoDadosPublicacaoValidos_QuandoAtualizarInformacoes_EntaoDeveAtualizarPropriedades()
        {
            // Arrange
            var sut = new CodafSuplementar(1);
            var dadosPublicacao = GerarDadosPublicacaoLista();

            // Act
            sut.AtualizarInformacoes(dadosPublicacao);

            // Assert
            sut.DataPublicacao.Should().Be(dadosPublicacao.DataPublicacao);
            sut.DataPublicacaoDom.Should().Be(dadosPublicacao.DataPublicacaoDom);
            sut.NumeroComunicado.Should().Be(dadosPublicacao.NumeroComunicado);
            sut.PaginaComunicadoDom.Should().Be(dadosPublicacao.PaginaComunicadoDom);
            sut.CodigoCursoEol.Should().Be(dadosPublicacao.CodigoCursoEol);
            sut.CodigoNivel.Should().Be(dadosPublicacao.CodigoNivel);
            sut.Observacao.Should().Be(dadosPublicacao.Observacao);
        }

        [Fact]
        public void DadoQualquerStatus_QuandoIniciar_EntaoDeveDefinirStatusComoIniciado()
        {
            // Arrange
            var sut = new CodafSuplementar(1);
            DefinirPropriedadePrivada(sut, "Status", StatusCodafSuplementar.Aguardando);

            // Act
            sut.Iniciar();

            // Assert
            sut.Status.Should().Be(StatusCodafSuplementar.Iniciado);
        }

        [Fact]
        public void DadoStatusFinalizado_QuandoDefinirStatus_EntaoDeveManterStatusFinalizado()
        {
            // Arrange
            var sut = new CodafSuplementar(1);
            DefinirPropriedadePrivada(sut, "Status", StatusCodafSuplementar.Finalizado);

            // Act
            sut.DefinirStatus();

            // Assert
            sut.Status.Should().Be(StatusCodafSuplementar.Finalizado);
        }

        [Fact]
        public void DadoTodosRequisitosPreenchidos_QuandoDefinirStatus_EntaoDeveDefinirStatusComoAguardando()
        {
            // Arrange
            var dadosPublicacao = GerarDadosPublicacaoListaValidos();
            var sut = new CodafSuplementar(1, dadosPublicacao)
            {
                CodafInscricoes = new List<CodafSuplementarInscricao> { mocker.CreateInstance<CodafSuplementarInscricao>() },
                CodafAnexos = new List<CodafSuplementarAnexo> { mocker.CreateInstance<CodafSuplementarAnexo>() }
            };

            // Act
            sut.DefinirStatus();

            // Assert
            sut.Status.Should().Be(StatusCodafSuplementar.Aguardando);
        }

        [Fact]
        public void DadoRequisitosIncompletos_QuandoDefinirStatus_EntaoDeveManterStatusAtual()
        {
            // Arrange
            var dadosPublicacao = GerarDadosPublicacaoListaValidos();
            var sut = new CodafSuplementar(1, dadosPublicacao)
            {
                CodafInscricoes = new List<CodafSuplementarInscricao>() // Requisito faltante: lista vazia
            };
            var statusInicial = sut.Status;

            // Act
            sut.DefinirStatus();

            // Assert
            sut.Status.Should().Be(statusInicial);
        }

        // ================= HELPER BOGUS E REFLECTION ================= //

        private static DadosPublicacaoLista GerarDadosPublicacaoLista() => new Faker<DadosPublicacaoLista>("pt_BR")
            .CustomInstantiator(f => new DadosPublicacaoLista(
                f.Date.Recent(),
                f.Date.Recent(),
                f.Random.Short(1, 1000),
                f.Random.Short(1, 100),
                f.Random.Int(1, 9999),
                f.Random.Int(1, 10),
                f.Lorem.Sentence()
            )).Generate();

        private static DadosPublicacaoLista GerarDadosPublicacaoListaValidos() => new Faker<DadosPublicacaoLista>("pt_BR")
            .CustomInstantiator(f => new DadosPublicacaoLista(
                f.Date.Recent(),
                f.Date.Recent(),
                f.Random.Short(1, 1000),
                f.Random.Short(1, 100),
                f.Random.Int(1, 9999),
                f.Random.Int(1, 10),
                f.Lorem.Sentence()
            )).Generate();

        private static void DefinirPropriedadePrivada<T>(T objeto, string nomePropriedade, object valor)
        {
            var propriedade = typeof(T).GetProperty(nomePropriedade, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            propriedade?.SetValue(objeto, valor);
        }
    }
}