using FluentAssertions;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Domino.Teste.Entidades
{
    public class CodafListaPresencaTests
    {
        private const long PROPOSTA_ID = 100;
        private const long PROPOSTA_TURMA_ID = 200;
        private const int CODIGO_CURSO_EOL = 12345;
        private const int CODIGO_NIVEL = 5;

        [Fact]
        public void DadoUsuarioComPerfilAdminDf_QuandoInstanciarEntidade_EntaoDevePreencherCodigosRestritos()
        {
            // Arrange
            var idPerfilAdmin = Perfis.ADMIN_DF;
            var dataPublicacao = new DateOnly(2024, 1, 1);

            // Act
            var entidade = new CodafListaPresenca(
                PROPOSTA_ID,
                PROPOSTA_TURMA_ID,
                dataPublicacao,
                null,
                1,
                null,
                CODIGO_CURSO_EOL,
                CODIGO_NIVEL,
                "Obs",
                idPerfilAdmin
            );

            // Assert
            entidade.PropostaId.Should().Be(PROPOSTA_ID);
            entidade.CodigoCursoEol.Should().Be(CODIGO_CURSO_EOL);
            entidade.CodigoNivel.Should().Be(CODIGO_NIVEL);
        }

        [Fact]
        public void DadoUsuarioSemPerfilAdminDf_QuandoInstanciarEntidade_EntaoCodigosRestritosDevemSerNulos()
        {
            // Arrange
            var idPerfilComum = Guid.NewGuid();

            // Act
            var entidade = new CodafListaPresenca(
                PROPOSTA_ID,
                PROPOSTA_TURMA_ID,
                null,
                null,
                null,
                null,
                CODIGO_CURSO_EOL,
                CODIGO_NIVEL,
                null,
                idPerfilComum
            );

            // Assert
            entidade.PropostaId.Should().Be(PROPOSTA_ID);
            entidade.CodigoCursoEol.Should().BeNull();
            entidade.CodigoNivel.Should().BeNull();
        }

        [Fact]
        public void DadoUsuarioAdminDf_QuandoAtualizarInformacoes_EntaoDeveAtualizarTodosOsCampos()
        {
            // Arrange
            var entidade = CriarEntidadePadrao();
            var idPerfilAdmin = Perfis.ADMIN_DF;
            var novoCodigoCurso = 99999;

            // Act
            entidade.AtualizarInformacoes(
                new DateOnly(2024, 2, 1),
                null,
                2,
                null,
                novoCodigoCurso,
                10,
                "Nova Obs",
                idPerfilAdmin
            );

            // Assert
            entidade.NumeroComunicado.Should().Be(2);
            entidade.CodigoCursoEol.Should().Be(novoCodigoCurso);
            entidade.Observacao.Should().Be("Nova Obs");
        }

        [Fact]
        public void DadoUmUsuarioComum_QuandoAtualizarInformacoes_EntaoNaoDeveAlterarCodigosRestritos()
        {
            // Arrange
            var entidade = new CodafListaPresenca(
                PROPOSTA_ID,
                PROPOSTA_TURMA_ID,
                null, null, null, null,
                CODIGO_CURSO_EOL,
                CODIGO_NIVEL,
                null,
                Perfis.ADMIN_DF
            );

            var idPerfilComum = Guid.NewGuid();
            var tentativaNovoCodigo = 88888;

            // Act
            entidade.AtualizarInformacoes(
                new DateOnly(2024, 3, 1),
                null,
                null,
                null,
                tentativaNovoCodigo,
                20,
                "Update Comum",
                idPerfilComum
            );

            // Assert
            entidade.Observacao.Should().Be("Update Comum");
            entidade.CodigoCursoEol.Should().Be(CODIGO_CURSO_EOL);
            entidade.CodigoCursoEol.Should().NotBe(tentativaNovoCodigo);
        }

        [Fact]
        public void DadoNovaEntidade_QuandoIniciar_EntaoStatusDeveSerAlteradoParaIniciado()
        {
            // Arrange
            var entidade = CriarEntidadePadrao();
            entidade.Status.Should().NotBe(StatusCodafListaPresenca.Iniciado);

            // Act
            entidade.Iniciar();

            // Assert
            entidade.Status.Should().Be(StatusCodafListaPresenca.Iniciado);
        }

        private static CodafListaPresenca CriarEntidadePadrao()
        {
            return new CodafListaPresenca(
                PROPOSTA_ID,
                PROPOSTA_TURMA_ID,
                DateOnly.FromDateTime(DateTime.Now),
                null,
                1,
                null,
                null,
                null,
                "Teste",
                Guid.NewGuid()
            );
        }
    }
}