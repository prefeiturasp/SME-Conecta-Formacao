using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.Api.Base
{
    [CollectionDefinition("WebApi Conecta Teste Integracao")]
    public class ConectaApiTestCollection : ICollectionFixture<ConectaWebApplicationFactory>
    {
    }
}
