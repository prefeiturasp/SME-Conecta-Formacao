using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Ues;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.Ues;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Ues
{
    public class CasoDeUsoObterAutocompletarNomeUe(IRepositorioUe repositorioUe) : ICasoDeUsoObterAutocompletarNomeUe
    {
        public async Task<Resultado<PaginacaoResultadoDto<AutocompletarNomeUeDto>>> ExecutarAsync(FiltroAutocompletarNomeUeDto filtro)
        {
            var resultado = await repositorioUe.AutocompletarNomeAsync(filtro.TermoBusca ?? "", filtro.DreId, filtro.NumeroPagina, filtro.NumeroRegistros);
            return new PaginacaoResultadoDto<AutocompletarNomeUeDto>(resultado.Itens, resultado.TotalRegistros, resultado.TotalPaginas);
        }
    }
}
