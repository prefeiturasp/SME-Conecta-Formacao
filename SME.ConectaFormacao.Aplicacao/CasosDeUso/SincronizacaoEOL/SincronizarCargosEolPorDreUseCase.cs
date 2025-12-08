using SME.ConectaFormacao.Aplicacao.Interfaces.SincronizacaoEOL;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.SincronizacaoEOL
{
    public class SincronizarCargosEolPorDreUseCase(IServicoEol servicoEol, IRepositorioSincronizador repositorioSincronizador) : ISincronizarCargosEolPorDreUseCase
    {
        public async Task<bool> Executar(MensagemRabbit param)
        {
            var codigoDre = param.ObterObjetoMensagem<string>() ?? 
                            throw new ArgumentNullException(nameof(param), "Parâmetro código DRE não pode ser nulo.");

            var cargosEolOrigem = await servicoEol.ObterCargosEolPorDreAsync(codigoDre);
            var cargosEolDestino = cargosEolOrigem?.Select(c => new CargoEol(
                c.CdCargo,
                c.CdRegistroFuncional,
                c.CodigoUe,
                c.Sobreposto,
                codigoDre)
                {
                    DataPosse = c.DataPosse is not null ? DateOnly.FromDateTime(c.DataPosse.Value) : null,
                    NomeCargo = c.NomeCargo,
                    TipoVinculo = c.TipoVinculo
                }).DistinctBy(c => c.ObterChaveNegocio()).ToList() ?? [];

            await repositorioSincronizador.SincronizarLoteCargosEolAsync(cargosEolDestino, codigoDre);
            return true;
        }
    }
}
