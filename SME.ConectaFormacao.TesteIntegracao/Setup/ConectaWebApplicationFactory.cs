using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Infra.Servicos.Options;
using SME.ConectaFormacao.TesteIntegracao.Api.Base;
using Testcontainers.Minio;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;
using Xunit;
namespace SME.ConectaFormacao.TesteIntegracao.Setup
{
    public class ConectaWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _pgContainer = new PostgreSqlBuilder("postgres:15.6")
            .WithDatabase("sme_conecta")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        private readonly RabbitMqContainer _rabbitContainer = new RabbitMqBuilder("rabbitmq:3.12.2-management")
            .WithPortBinding(5672, 5672)
            .WithUsername("admin")
            .WithPassword("admin")
            .Build();

        private readonly MinioContainer _minioContainer = new MinioBuilder("minio/minio:RELEASE.2022-01-28T02-28-16Z")
            .WithUsername("sme_admin")
            .WithPassword("sme_secret_123")
            .Build();

        private readonly RedisContainer _redisContainer = new RedisBuilder("redis:7.2")
            .Build();

        public string StringDeConexaoPostgres => _pgContainer.GetConnectionString();
        public string StringDeConexaoRedis => _redisContainer.GetConnectionString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                var diretorioTestes = AppContext.BaseDirectory;
                var caminhoArquivoJson = Path.Combine(diretorioTestes, "appsettings.Testing.json");
                config.AddJsonFile(caminhoArquivoJson, optional: false, reloadOnChange: true);
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:conexao"] = _pgContainer.GetConnectionString(),

                    ["Redis:EndPoint"] = _redisContainer.GetConnectionString(),

                    ["ConfiguracaoArmazenamento:EndPoint"] = _minioContainer.Hostname,
                    ["ConfiguracaoArmazenamento:Port"] = _minioContainer.GetMappedPublicPort(9000).ToString(),

                    [$"{ConfiguracaoRabbitOptions.Secao}:HostName"] = _rabbitContainer.Hostname,
                    [$"{ConfiguracaoRabbitOptions.Secao}:Porta"] = _rabbitContainer.GetMappedPublicPort(5672).ToString(),
                    [$"{ConfiguracaoRabbitOptions.Secao}:UserName"] = "admin",
                    [$"{ConfiguracaoRabbitOptions.Secao}:Password"] = "admin",
                    [$"{ConfiguracaoRabbitOptions.Secao}:VirtualHost"] = "/",

                    [$"{ConfiguracaoRabbitLogsOptions.Secao}:HostName"] = _rabbitContainer.Hostname,
                    [$"{ConfiguracaoRabbitLogsOptions.Secao}:Porta"] = _rabbitContainer.GetMappedPublicPort(5672).ToString(),
                    [$"{ConfiguracaoRabbitLogsOptions.Secao}:UserName"] = "admin",
                    [$"{ConfiguracaoRabbitLogsOptions.Secao}:Password"] = "admin",
                    [$"{ConfiguracaoRabbitLogsOptions.Secao}:VirtualHost"] = "/"
                });
            });

            builder.ConfigureTestServices(services =>
            {
                services.AddTransient<TestAuthHandler>();
                services.PostConfigure<AuthenticationOptions>(options =>
                {
                    var jwtScheme = options.Schemes.FirstOrDefault(s => s.Name == JwtBearerDefaults.AuthenticationScheme);

                    if (jwtScheme != null)
                    {
                        jwtScheme.HandlerType = typeof(TestAuthHandler);
                    }
                    else
                    {
                        options.AddScheme(JwtBearerDefaults.AuthenticationScheme, scheme =>
                        {
                            scheme.HandlerType = typeof(TestAuthHandler);
                        });
                    }
                });
                services.PostConfigure<ConfiguracaoRabbitOptions>(options =>
                {
                    options.HostName = _rabbitContainer.Hostname;
                    options.UserName = "admin";
                    options.Password = "admin";
                    options.VirtualHost = "/";
                });
                services.PostConfigure<ConfiguracaoRabbitLogsOptions>(options =>
                {
                    options.HostName = _rabbitContainer.Hostname;
                    options.UserName = "admin";
                    options.Password = "admin";
                    options.VirtualHost = "/";
                });
            });
        }

        public async Task InitializeAsync()
        {
            await Task.WhenAll(
                _pgContainer.StartAsync(),
                _rabbitContainer.StartAsync(),
                _minioContainer.StartAsync(),
                _redisContainer.StartAsync()
            );


            Environment.SetEnvironmentVariable("ConnectionStrings__conexao", _pgContainer.GetConnectionString());

            Environment.SetEnvironmentVariable("Redis__EndPoint", _redisContainer.GetConnectionString());

            Environment.SetEnvironmentVariable("ConfiguracaoArmazenamento__EndPoint", _minioContainer.Hostname);
            Environment.SetEnvironmentVariable("ConfiguracaoArmazenamento__Port", _minioContainer.GetMappedPublicPort(9000).ToString());

            Environment.SetEnvironmentVariable("ConfiguracaoRabbit__Hostname", _rabbitContainer.Hostname);
            Environment.SetEnvironmentVariable("ConfiguracaoRabbit__Porta", _rabbitContainer.GetMappedPublicPort(5672).ToString());

            Environment.SetEnvironmentVariable("ConfiguracaoRabbitLog__Hostname", _rabbitContainer.Hostname);
            Environment.SetEnvironmentVariable("ConfiguracaoRabbitLog__Porta", _rabbitContainer.GetMappedPublicPort(5672).ToString());

            MigradorDeTabelas.Migrar(StringDeConexaoPostgres);
        }

        public new async Task DisposeAsync()
        {
            await Task.WhenAll(
                _pgContainer.DisposeAsync().AsTask(),
                _rabbitContainer.DisposeAsync().AsTask(),
                _minioContainer.DisposeAsync().AsTask(),
                _redisContainer.DisposeAsync().AsTask()
            );
        }
    }
}