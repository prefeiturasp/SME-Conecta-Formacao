using Bogus;
using SME.ConectaFormacao.Dominio.Comparadores;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Domino.Teste.Comparadores
{
    public class UsuarioAcessibilidadeComparadorDadosTests
    {
        private readonly UsuarioAcessibilidadeComparadorDados _comparador;
        private readonly Faker _faker;

        public UsuarioAcessibilidadeComparadorDadosTests()
        {
            _comparador = UsuarioAcessibilidadeComparadorDados.Instancia;
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public void DadoMesmaReferenciaDeObjeto_QuandoComparar_EntaoDeveRetornarVerdadeiro()
        {
            // Arrange
            var objetoA = GerarUsuarioAcessibilidade();
            var objetoB = objetoA;

            // Act
            var resultado = _comparador.Equals(objetoA, objetoB);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public void DadoUmObjetoNulo_QuandoCompararComNaoNulo_EntaoDeveRetornarFalso()
        {
            // Arrange
            var objetoA = GerarUsuarioAcessibilidade();
            UsuarioAcessibilidade? objetoB = null;

            // Act
            var resultado = _comparador.Equals(objetoA, objetoB);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public void DadoObjetosComMesmosDadosDeNegocio_MasIdsEExcluidoDiferentes_QuandoComparar_EntaoDeveRetornarVerdadeiro()
        {
            // Arrange
            var usuarioId = _faker.Random.Long(1);
            var descricao = "Deficiência Visual";

            var objetoA = new UsuarioAcessibilidade
            {
                Id = 10,
                UsuarioId = usuarioId,
                PossuiDeficiencia = true,
                DescricaoDeficiencia = descricao,
                NecessitaAdaptacao = false,
                Excluido = false
            };

            var objetoB = new UsuarioAcessibilidade
            {
                Id = 999,
                UsuarioId = usuarioId,
                PossuiDeficiencia = true,
                DescricaoDeficiencia = descricao,
                NecessitaAdaptacao = false,
                Excluido = true
            };

            // Act
            var resultado = _comparador.Equals(objetoA, objetoB);

            // Assert
            Assert.True(resultado, "O comparador deve ignorar diferenças em Id e Excluido.");
        }

        [Fact]
        public void DadoDiferencaEmPropriedadeDeNegocio_QuandoComparar_EntaoDeveRetornarFalso()
        {
            // Arrange
            var objetoA = GerarUsuarioAcessibilidade();

            var objetoB = new UsuarioAcessibilidade
            {
                UsuarioId = objetoA.UsuarioId,
                PossuiDeficiencia = !objetoA.PossuiDeficiencia,
                DescricaoDeficiencia = objetoA.DescricaoDeficiencia,
                NecessitaAdaptacao = objetoA.NecessitaAdaptacao,
                DescricaoAdaptacao = objetoA.DescricaoAdaptacao
            };

            // Act
            var resultado = _comparador.Equals(objetoA, objetoB);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public void DadoStringsComFormatacaoDiferente_QuandoComparar_EntaoDeveNormalizarERetornarVerdadeiro()
        {
            // Arrange
            var objetoA = new UsuarioAcessibilidade
            {
                UsuarioId = 1,
                DescricaoDeficiencia = "  BAIXA VISÃO  ",
                DescricaoAdaptacao = "LUPA"
            };

            var objetoB = new UsuarioAcessibilidade
            {
                UsuarioId = 1,
                DescricaoDeficiencia = "baixa visão",
                DescricaoAdaptacao = "  lupa  "
            };

            // Act
            var resultado = _comparador.Equals(objetoA, objetoB);

            // Assert
            Assert.True(resultado, "Deve ignorar espaços em branco e diferenciação de maiúsculas/minúsculas.");
        }

        [Fact]
        public void DadoObjetosConsideradosIguais_QuandoGerarHashCode_EntaoDevemSerIdenticos()
        {
            // Arrange
            var objetoA = new UsuarioAcessibilidade
            {
                UsuarioId = 55,
                DescricaoDeficiencia = "Auditiva",
                Excluido = false
            };

            var objetoB = new UsuarioAcessibilidade
            {
                UsuarioId = 55,
                DescricaoDeficiencia = "AUDITIVA",
                Excluido = true
            };

            // Act
            var hashA = _comparador.GetHashCode(objetoA);
            var hashB = _comparador.GetHashCode(objetoB);

            // Assert
            Assert.Equal(hashA, hashB);
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", null)]
        [InlineData(null, null)]
        public void DadoStringsNulasOuVazias_QuandoComparar_EntaoDeveConsiderarIguais(string? valorA, string? valorB)
        {
            // Arrange
            var objetoA = new UsuarioAcessibilidade { DescricaoDeficiencia = valorA };
            var objetoB = new UsuarioAcessibilidade { DescricaoDeficiencia = valorB };

            // Act
            var resultado = _comparador.Equals(objetoA, objetoB);

            // Assert
            Assert.True(resultado);
        }

        private UsuarioAcessibilidade GerarUsuarioAcessibilidade()
        {
            return new UsuarioAcessibilidade
            {
                UsuarioId = _faker.Random.Long(1),
                PossuiDeficiencia = _faker.Random.Bool(),
                DescricaoDeficiencia = _faker.Lorem.Word(),
                NecessitaAdaptacao = _faker.Random.Bool(),
                DescricaoAdaptacao = _faker.Lorem.Word(),
                Excluido = false
            };
        }
    }
}
