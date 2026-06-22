using SME.ConectaFormacao.Aplicacao.Servicos.Interfaces;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao.Servicos
{
    public class ServicoPeriodoEncontroProposta(
        IRepositorioPropostaEncontro repositorioPropostaEncontro,
        ICacheDistribuido cacheDistribuido) : IServicoPeriodoEncontroProposta
    {
        public async Task<string> ObterPeriodoEncontrosTurmaAsync(long turmaId)
        {
            var datasInicio = new List<DateTime>();
            var datasFim = new List<DateTime>();

            var encontros = await cacheDistribuido.ObterAsync(CacheDistribuidoNomes.PropostaTurmaEncontro.Parametros(turmaId),
                () => repositorioPropostaEncontro.ObterEncontrosPorPropostaTurmaAsync(turmaId));

            foreach (var encontro in encontros)
            {
                foreach (var data in encontro.Datas)
                {
                    if (data.DataInicio.HasValue)
                        datasInicio.Add(data.DataInicio.Value);

                    if (data.DataFim.HasValue)
                        datasFim.Add(data.DataFim.Value);
                }
            }

            var menorDataInicio = datasInicio.OrderBy(o => o.Date).FirstOrDefault();
            DateTime? maiorDataFim = null;
            if (datasFim.NaoPossuiElementos() && datasInicio.Count > 1)
            {
                maiorDataFim = datasInicio.OrderBy(o => o.Date).LastOrDefault();
            }
            else if (datasFim.PossuiElementos())
            {
                maiorDataFim = datasFim.OrderBy(o => o.Date).LastOrDefault();
            }

            return maiorDataFim != null ? $" {menorDataInicio:dd/MM/yyyy} até {maiorDataFim:dd/MM/yyyy}" : $" {menorDataInicio:dd/MM/yyyy}";
        }
    }
}
