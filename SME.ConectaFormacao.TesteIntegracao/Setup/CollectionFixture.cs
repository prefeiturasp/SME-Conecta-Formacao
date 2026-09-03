using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.Setup
{
    public class CollectionFixture
    {
        public CollectionFixture()
        {
        }
    }

    [CollectionDefinition("TesteIntegradoConectaFormacao")]
    public class CollectionDoTeste : ICollectionFixture<CollectionFixture>
    {
    }
}
