using Elastic.Apm.StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Infra.Servicos.Cache;
using SME.ConectaFormacao.Infra.Servicos.CacheDistribuido.Opcoes;
using StackExchange.Redis;

namespace SME.ConectaFormacao.Infra.Servicos.CacheDistribuido.IoC
{
    public static class CacheDistribuidoRedisCHelper
    {
        public static void ConfigurarCacheDistribuidoRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var cacheDistribuidoRedisOpcao = new CacheDistribuidoRedisOpcao();
            configuration.GetSection(CacheDistribuidoRedisOpcao.Secao).Bind(cacheDistribuidoRedisOpcao, c => c.BindNonPublicProperties = true);
            services.AddSingleton(cacheDistribuidoRedisOpcao);

            var redisConfigurationOptions = new ConfigurationOptions()
            {
                Proxy = cacheDistribuidoRedisOpcao.Proxy,
                SyncTimeout = cacheDistribuidoRedisOpcao.SyncTimeout,
                EndPoints = { cacheDistribuidoRedisOpcao.Endpoint }
            };

            var muxer = ConnectionMultiplexer.Connect(redisConfigurationOptions);
            services.AddSingleton<IConnectionMultiplexer>(muxer);

            services.AddSingleton<ICacheDistribuido, CacheDistribuidoRedis>();

            muxer.UseElasticApm();
        }
    }
}
