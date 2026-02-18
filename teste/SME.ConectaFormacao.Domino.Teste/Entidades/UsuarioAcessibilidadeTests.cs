using Bogus;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Domino.Teste.Entidades
{
    public class UsuarioAcessibilidadeTests
    {
        private readonly Faker _faker;

        public UsuarioAcessibilidadeTests()
        {
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public void DadoDoisObjetosComMesmosValores_QuandoComparar_EntaoDeveRetornarVerdadeiro()
        {
            // Arrange
            var descricao = _faker.Lorem.Sentence();
            var usuarioId = _faker.Random.Long(1);

            var objetoA = new UsuarioAcessibilidade
            {
                UsuarioId = usuarioId,
                PossuiDeficiencia = true,
                DescricaoDeficiencia = descricao,
                NecessitaAdaptacao = false,
                DescricaoAdaptacao = null,
                Excluido = false
            };

            var objetoB = new UsuarioAcessibilidade
            {
                UsuarioId = usuarioId,
                PossuiDeficiencia = true,
                DescricaoDeficiencia = descricao,
                NecessitaAdaptacao = false,
                DescricaoAdaptacao = null,
                Excluido = false
            };

            // Act
            var resultadoEquals = objetoA.Equals(objetoB);
            var resultadoOperator = objetoA == objetoB;

            // Assert
            Assert.True(resultadoEquals);
            Assert.True(resultadoOperator);
        }

        [Fact]
        public void DadoObjetosComStringsDiferentesApenasPorCaixaAltaOuEspaco_QuandoComparar_EntaoDeveRetornarVerdadeiro()
        {
            // Arrange
            var objetoA = new UsuarioAcessibilidade
            {
                UsuarioId = 1,
                DescricaoDeficiencia = "DEFICIÊNCIA VISUAL"
            };

            var objetoB = new UsuarioAcessibilidade
            {
                UsuarioId = 1,
                DescricaoDeficiencia = "  deficiência visual  "
            };

            // Act
            var resultado = objetoA.Equals(objetoB);

            // Assert
            Assert.True(resultado, "O método deve ignorar Case Sensitive e realizar Trim nas strings.");
        }

        [Fact]
        public void DadoStringNulaEStringVazia_QuandoComparar_EntaoDeveConsiderarIguais()
        {
            // Arrange
            var objetoA = new UsuarioAcessibilidade { DescricaoAdaptacao = null };
            var objetoB = new UsuarioAcessibilidade { DescricaoAdaptacao = "" };

            // Act
            var resultado = objetoA.Equals(objetoB);

            // Assert
            Assert.True(resultado, "Nulo e Vazio devem ser considerados equivalentes nesta regra de negócio.");
        }

        [Fact]
        public void DadoObjetosComValoresDiferentes_QuandoComparar_EntaoDeveRetornarFalso()
        {
            // Arrange
            var objetoA = new UsuarioAcessibilidade { UsuarioId = 1, PossuiDeficiencia = true };
            var objetoB = new UsuarioAcessibilidade { UsuarioId = 1, PossuiDeficiencia = false };

            // Act
            var resultadoEquals = objetoA.Equals(objetoB);
            var resultadoOperator = objetoA != objetoB;

            // Assert
            Assert.False(resultadoEquals);
            Assert.True(resultadoOperator);
        }

        [Fact]
        public void DadoDiferencaNaPropriedadeExcluido_QuandoComparar_EntaoDeveRetornarFalso()
        {
            // Arrange
            var objetoA = new UsuarioAcessibilidade { UsuarioId = 1, Excluido = false };
            var objetoB = new UsuarioAcessibilidade { UsuarioId = 1, Excluido = true };

            // Act
            var resultado = objetoA.Equals(objetoB);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public void DadoUmObjeto_QuandoCompararComNulo_EntaoDeveRetornarFalso()
        {
            // Arrange
            var objetoA = new UsuarioAcessibilidade();
            UsuarioAcessibilidade? objetoB = null;

            // Act
            var resultado = objetoA.Equals(objetoB);
            var resultadoOperator = objetoA == null;

            // Assert
            Assert.False(resultado);
            Assert.False(resultadoOperator);
        }

        [Fact]
        public void DadoDoisObjetosIguais_QuandoObterHashCode_EntaoDevemSerIdenticos()
        {
            // Arrange
            var objetoA = new UsuarioAcessibilidade
            {
                UsuarioId = 10,
                DescricaoDeficiencia = "Auditiva "
            };

            var objetoB = new UsuarioAcessibilidade
            {
                UsuarioId = 10,
                DescricaoDeficiencia = "auditiva"
            };

            // Act
            var hashA = objetoA.GetHashCode();
            var hashB = objetoB.GetHashCode();

            // Assert
            Assert.Equal(hashA, hashB);
        }
    }
}
