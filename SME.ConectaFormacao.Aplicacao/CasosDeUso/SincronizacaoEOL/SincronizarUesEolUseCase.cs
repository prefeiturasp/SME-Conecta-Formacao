using SME.ConectaFormacao.Aplicacao.Interfaces.SincronizacaoEOL;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Log;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.SincronizacaoEOL
{
    public class SincronizarUesEolUseCase(
        IServicoEol servicoEol,
        IServicoLogs servicoLogs,
        IRepositorioSincronizador repositorioSincronizador,
        IRepositorioDre repositorioDre) : ISincronizarUesEolUseCase
    {
        public async Task<bool> Executar(MensagemRabbit param)
        {
            try
            {
                await servicoLogs.Enviar($"Iniciando sincronização de UEs EOL", LogContexto.SincronizacaoUesEol, LogNivel.Informacao);
                var uesEolOrigem = await servicoEol.ObterTodasAsUesAsync();
                var dreIdCodigoDre = await ObterDreIdCodigoDreAsync(uesEolOrigem);
                var uesEolDestino = new List<Ue>();

                if (uesEolOrigem == null || !uesEolOrigem.Any())
                {
                    await servicoLogs.Enviar($"Nenhuma UE encontrada para sincronização", LogContexto.SincronizacaoUesEol, LogNivel.Alerta);
                    return true;
                }

                foreach (var ueEol in uesEolOrigem)
                {
                    if (!dreIdCodigoDre.TryGetValue(ueEol.CodigoDRE, out var dreId))
                    {
                        await servicoLogs.Enviar($"DRE com código {ueEol.CodigoDRE} não encontrada para UE {ueEol.CodigoEscola}", LogContexto.SincronizacaoUesEol, LogNivel.Alerta);
                        continue;
                    }

                    uesEolDestino.Add(new Ue
                    {
                        DreId = dreId,
                        CodigoUe = ueEol.CodigoEscola,
                        NomeEscola = ueEol.NomeEscola,
                        TipoEscola = ueEol.CodigoTipoEscola,
                        SiglaTipoEscola = ueEol.SiglaTipoEscola
                    });
                }

                await repositorioSincronizador.SincronizarLoteUeEolAsync(uesEolDestino);
                return true;
            }
            catch (Exception ex)
            {
                await servicoLogs.Enviar($"Erro ao sincronizar UEs EOL: {ex.Message}", LogContexto.SincronizacaoUesEol, LogNivel.Critico);
                return false;
            }
        }

        private async Task<Dictionary<string, long>> ObterDreIdCodigoDreAsync(IEnumerable<UeEol>? uesEol)
        {
            var dict = new Dictionary<string, long>();
            var codigoDres = uesEol?.Select(u => u.CodigoDRE).Distinct().ToList();
            if (codigoDres == null || codigoDres.Count == 0)
                return dict;
            foreach (var codigoDre in codigoDres)
            {
                var dre = await repositorioDre.ObterDrePorCodigo(codigoDre);
                if (dre != null)
                    dict.Add(codigoDre, dre.Id);
            }
            return dict;
        }
    }
}
