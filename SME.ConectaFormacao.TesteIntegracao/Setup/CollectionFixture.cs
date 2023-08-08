using Dapper.FluentMap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Globalization;
using System.Text;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.Setup
{
    public class CollectionFixture : IDisposable
    {
        public IServiceCollection Services { get; set; }
        public InMemoryDatabase Database { get; }
        public ServiceProvider ServiceProvider { get; set; }

        public CollectionFixture()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Database = new InMemoryDatabase();

            IniciarServicos();

        }

        public void IniciarServicos()
        {
            Services = new ServiceCollection();

            Services.AddScoped<IDbConnection>(x => Database.Conexao);

            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", false).Build();

            Services.AddSingleton<IConfiguration>(config);
            Services.AddMemoryCache();

            FluentMapper.EntityMaps.Clear();

            var culture = CultureInfo.CreateSpecificCulture("pt-BR");

            CultureInfo.CurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            new RegistradorDependencias(Services, config).Registrar();
        }

        public void BuildServiceProvider()
        {
            ServiceProvider = Services.BuildServiceProvider();
            //DapperExtensionMethods.Init(ServiceProvider.GetService<IServicoTelemetria>()); //TODO: Será usado o Auto Instrumentation do APM
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }

    [CollectionDefinition("TesteIntegradoConectaFormacao")]
    public class CollectionDoTeste : ICollectionFixture<CollectionFixture>
    {
    }
}
