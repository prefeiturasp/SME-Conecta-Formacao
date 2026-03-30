using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
namespace SME.ConectaFormacao.TesteIntegracao.Setup
{
    public class ConectaWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        public Task InitializeAsync()
        {
            throw new NotImplementedException();
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
