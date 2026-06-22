using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.Propostas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterAutocompletarFormacao(
        IRepositorioProposta repositorioProposta) : ICasoDeUsoObterAutocompletarFormacao
    {
        public async Task<Resultado<PaginacaoResultadoDto<AutocompletarNumeroHomologacaoDto>>> ExecutarAsync(FiltroAutocompletarNumeroHomologacaoDto filtro)
        {
            if (string.IsNullOrWhiteSpace(filtro.TermoBusca))
                return new PaginacaoResultadoDto<AutocompletarNumeroHomologacaoDto>([], 0, 0);

            var resultado = await repositorioProposta.ObterAutocompletarNumeroHomologacaoAsync(filtro.TermoBusca, filtro.ComCodaf, filtro.NumeroPagina, filtro.NumeroRegistros);
            return new PaginacaoResultadoDto<AutocompletarNumeroHomologacaoDto>(resultado.Itens, resultado.TotalRegistros, resultado.TotalPaginas);
        }
    }
}
