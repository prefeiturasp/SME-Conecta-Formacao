using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares
{
    public class CasoDeUsoListarCodafSuplementar(
        IRepositorioCodafSuplementar repositorioCodafSuplementar,
        IMapper mapper) : ICasoDeUsoListarCodafSuplementar
    {
        public async Task<Resultado<PaginacaoResultadoDto<CodafSuplementarResumoDto>>> ExecutarAsync(FiltroCodafSuplementarDto filtro)
        {
            var filtroRepositorio = mapper.Map<FiltroListagemResultadoCodafSuplementarDto>(filtro);
            var resultado = await repositorioCodafSuplementar.ObterListagemResultadoCodafSuplementarPorFiltroAsync(filtroRepositorio);
            var resultadoDto = new PaginacaoResultadoDto<CodafSuplementarResumoDto>(
                mapper.Map<List<CodafSuplementarResumoDto>>(resultado.Itens),
                resultado.TotalRegistros,
                resultado.TamanhoPagina);
            return resultadoDto;
        }
    }
}
