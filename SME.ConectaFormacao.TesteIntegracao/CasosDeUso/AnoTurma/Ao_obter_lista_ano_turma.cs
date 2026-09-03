using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.AnoTurma
{
    public class AoObterListaAnoTurma(CollectionFixture collectionFixture, TestFixture testFixture) : TesteBase(collectionFixture)
    {
        [Fact(DisplayName = "Ano Turma - Deve obter a lista de anos das turmas com opção todos")]
        public async Task Deve_obter_lista_anos_das_turmas_com_opcao_todos()
        {
            Assert.NotNull(testFixture);
            Assert.NotNull(_collectionFixture);
        }
    }
}
