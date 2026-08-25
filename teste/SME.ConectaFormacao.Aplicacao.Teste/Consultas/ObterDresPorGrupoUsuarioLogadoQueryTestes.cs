namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterDresPorGrupoUsuarioLogadoQueryTestes
    {
        [Fact]
        public void Instancia_Deve_Retornar_Uma_Instancia_Da_Query()
        {
            // Act
            var query = ObterDresPorGrupoUsuarioLogadoQuery.Instancia();

            // Assert
            Assert.NotNull(query);
            Assert.IsType<ObterDresPorGrupoUsuarioLogadoQuery>(query);
        }

        [Fact]
        public void Instancia_Deve_Sempre_Retornar_A_Mesma_Instancia()
        {
            // Act
            var primeiraInstancia = ObterDresPorGrupoUsuarioLogadoQuery.Instancia();
            var segundaInstancia = ObterDresPorGrupoUsuarioLogadoQuery.Instancia();

            // Assert
            Assert.Same(primeiraInstancia, segundaInstancia);
        }
    }
}
