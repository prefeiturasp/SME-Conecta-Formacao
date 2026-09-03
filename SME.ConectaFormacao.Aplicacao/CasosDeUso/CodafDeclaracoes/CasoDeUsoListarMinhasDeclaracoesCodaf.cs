using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafDeclaracoes;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafDeclaracoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafDeclaracoes
{
    public class CasoDeUsoListarMinhasDeclaracoesCodaf(
        IRepositorioCodafDeclaracao repositorioCodafDeclaracao,
        IMapper mapper) : ICasoDeUsoListarMinhasDeclaracoesCodaf
    {
        public async Task<Resultado<PaginacaoResultadoDto<MinhasDeclaracoesCodafDto>>> ExecutarAsync(FiltroListaMinhasDeclaracoesCodafDto filtro)
        {
            var filtroRepositorio = mapper.Map<FiltroMinhasDeclaracoesCodafDto>(filtro);
            var resultado = await repositorioCodafDeclaracao.ObterMinhasDeclaracoesPorFiltroAsync(filtroRepositorio);
            var resultadoDto = new PaginacaoResultadoDto<MinhasDeclaracoesCodafDto>(
                resultado.Itens,
                resultado.TotalRegistros,
                resultado.TamanhoPagina);
            return resultadoDto;
        }
    }
}
