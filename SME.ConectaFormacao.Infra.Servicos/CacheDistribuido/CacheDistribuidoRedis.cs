using SME.ConectaFormacao.Infra.Servicos.CacheDistribuido.Opcoes;
using SME.ConectaFormacao.Infra.Servicos.Log;
using StackExchange.Redis;

namespace SME.ConectaFormacao.Infra.Servicos.CacheDistribuido
{
    public class CacheDistribuidoRedis : CacheDistribuido
    {
        private readonly IDatabase _redis;
        private readonly CacheDistribuidoRedisOpcao _cacheDistribuidoRedisOpcao;

        public CacheDistribuidoRedis(IServicoLogs servicoLogs, IConnectionMultiplexer connectionMultiplexer, CacheDistribuidoRedisOpcao cacheDistribuidoRedisOpcao) : base(servicoLogs, "CacheDistribuidoRedis")
        {
            if (connectionMultiplexer == null) throw new ArgumentNullException(nameof(connectionMultiplexer));
            _cacheDistribuidoRedisOpcao = cacheDistribuidoRedisOpcao ?? throw new ArgumentNullException(nameof(cacheDistribuidoRedisOpcao));

            _redis = connectionMultiplexer.GetDatabase();
        }

        private string TratarNomeChave(string nomeChave) => string.Concat(_cacheDistribuidoRedisOpcao.Prefixo, nomeChave);

        protected override async Task<string> ObterValorAsync(string nomeChave)
        {
            return await _redis.StringGetAsync(TratarNomeChave(nomeChave));
        }

        protected override Task RemoverValorAsync(string nomeChave)
        {
            return _redis.KeyDeleteAsync(TratarNomeChave(nomeChave));
        }

        protected override Task SalvarValorAsync(string nomeChave, string valor, int minutosParaExpirar)
        {
            return _redis.StringSetAsync(new RedisKey(TratarNomeChave(nomeChave)), new RedisValue(valor), TimeSpan.FromMinutes(minutosParaExpirar));
        }
    }
}
