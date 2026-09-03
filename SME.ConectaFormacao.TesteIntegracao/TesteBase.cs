using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace SME.ConectaFormacao.TesteIntegracao
{
    [Collection("TesteIntegradoConectaFormacao")]
    public class TesteBase(CollectionFixture collectionFixture) : IClassFixture<TestFixture>
    {
        protected readonly CollectionFixture _collectionFixture = collectionFixture;
    }
}
