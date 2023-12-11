using StackExchange.Redis;

namespace SME.ConectaFormacao.Infra.Servicos.CacheDistribuido.Opcoes
{
    public class CacheDistribuidoRedisOpcao
    {
        public const string Secao = "Redis";

        public string Endpoint { get; set; }
        public Proxy Proxy { get; set; }
        public int SyncTimeout { get; set; } = 5000;
        public string Prefixo { get; set; } = "CONECTA:";
    }
}
