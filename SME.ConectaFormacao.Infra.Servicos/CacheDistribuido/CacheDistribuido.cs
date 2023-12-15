using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Cache;
using SME.ConectaFormacao.Infra.Servicos.Log;
using SME.ConectaFormacao.Infra.Servicos.Utilitarios;

namespace SME.ConectaFormacao.Infra.Servicos.CacheDistribuido
{
    public abstract class CacheDistribuido : ICacheDistribuido
    {
        private readonly IServicoLogs _servicoLogs;

        protected CacheDistribuido(IServicoLogs servicoLogs, string nomeServicoCache)
        {
            _servicoLogs = servicoLogs ?? throw new ArgumentNullException(nameof(servicoLogs));
            NomeServicoCache = nomeServicoCache ?? throw new ArgumentNullException(nameof(nomeServicoCache));
        }

        protected string NomeServicoCache { get; set; }

        protected virtual Task<string> ObterValorAsync(string nomeChave)
            => throw new NotImplementedException($"Método ObterValor do serviço {NomeServicoCache} não implementado");

        protected virtual Task RemoverValorAsync(string nomeChave)
            => throw new NotImplementedException($"Método RemoverValor do serviço {NomeServicoCache} não implementado");

        protected virtual Task SalvarValorAsync(string nomeChave, string valor, int minutosParaExpirar)
            => throw new NotImplementedException($"Método SalvarValor do serviço {NomeServicoCache} não implementado");

        public async Task<string> ObterAsync(string nomeChave, bool utilizarGZip = false)
        {
            try
            {
                var stringCache = await ObterValorAsync(nomeChave);

                if (string.IsNullOrWhiteSpace(stringCache))
                {
                    return await Task.FromResult(string.Empty);
                }

                if (utilizarGZip)
                    stringCache = Gzip.Descomprimir(Convert.FromBase64String(stringCache));

                return stringCache;
            }
            catch (Exception e)
            {
                await _servicoLogs.Enviar(
                    $"Erro ao obter async - {NomeServicoCache}",
                    LogContexto.Cache,
                    LogNivel.Alerta,
                    $"Nome chave: {nomeChave}",
                    e.StackTrace);

                return string.Empty;
            }
        }

        public async Task<T> ObterAsync<T>(string nomeChave, Func<Task<T>> buscarDados, int minutosParaExpirar = 720, bool utilizarGZip = false)
        {
            try
            {
                var stringCache = await ObterValorAsync(nomeChave);
                if (!string.IsNullOrWhiteSpace(stringCache))
                {
                    if (utilizarGZip)
                        stringCache = Gzip.Descomprimir(Convert.FromBase64String(stringCache));

                    return stringCache.JsonParaObjeto<T>();
                }

                var dados = await buscarDados();
                await SalvarAsync(nomeChave, dados, minutosParaExpirar, utilizarGZip);

                return dados;
            }
            catch (Exception e)
            {
                await _servicoLogs.Enviar(
                    $"Erro ao obter cache - {NomeServicoCache}",
                    LogContexto.Cache,
                    LogNivel.Alerta,
                    $"Nome chave: {nomeChave}",
                    e.StackTrace);

                return await buscarDados();
            }
        }

        public async Task<T> ObterObjetoAsync<T>(string nomeChave, bool utilizarGZip = false) where T : new()
        {
            try
            {
                var stringCache = await ObterValorAsync(nomeChave);
                if (string.IsNullOrWhiteSpace(stringCache))
                {
                    return default;
                }

                if (utilizarGZip)
                    stringCache = Gzip.Descomprimir(Convert.FromBase64String(stringCache));

                return stringCache.JsonParaObjeto<T>();
            }
            catch (Exception e)
            {
                await _servicoLogs.Enviar(
                    $"Erro ao obter objeto cache - {NomeServicoCache}",
                    LogContexto.Cache,
                    LogNivel.Alerta,
                    $"Nome chave: {nomeChave}",
                    e.StackTrace);

                return default;
            }
        }

        public async Task RemoverAsync(string nomeChave)
        {
            try
            {
                await RemoverValorAsync(nomeChave);
            }
            catch (Exception e)
            {
                await _servicoLogs.Enviar(
                     $"Erro ao remover cache - {NomeServicoCache}",
                     LogContexto.Cache,
                     LogNivel.Alerta,
                     $"Nome chave: {nomeChave}",
                     e.StackTrace);
            }
        }

        public async Task SalvarAsync(string nomeChave, string valor, int minutosParaExpirar = 720, bool utilizarGZip = false)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(valor) && valor != "[]" && valor != "null")
                {
                    if (utilizarGZip)
                    {
                        var valorComprimido = Gzip.Comprimir(valor);
                        valor = Convert.ToBase64String(valorComprimido);
                    }

                    await SalvarValorAsync(nomeChave, valor, minutosParaExpirar);
                }
            }
            catch (Exception e)
            {
                await _servicoLogs.Enviar(
                    $"Erro ao salvar cache - {NomeServicoCache}",
                    LogContexto.Cache,
                    LogNivel.Alerta,
                    $"Nome chave: {nomeChave}",
                    e.StackTrace);
            }
        }

        public Task SalvarAsync<T>(string nomeChave, T valor, int minutosParaExpirar = 720, bool utilizarGZip = false)
        {
           return SalvarAsync(nomeChave, valor.ObjetoParaJson(), minutosParaExpirar, utilizarGZip);
        }
    }
}
