using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafConferencias;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafConferencias;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafConferencias;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafConferencias
{
    public class CasoDeUsoListarCodafConferencia(
        IRepositorioCodafConferencia repositorio,
        IMapper mapper) : ICasoDeUsoListarCodafConferencia
    {
        public async Task<Resultado<PaginacaoResultadoDto<CodafConferenciaResumoDto>>> ExecutarAsync(FiltroCodafConferenciaDto filtro)
        {
            var filtroRepositorio = mapper.Map<FiltroListagemResultadoCodafConferenciaDto>(filtro);
            var resultado = await repositorio.ObterListagemPorFiltroAsync(filtroRepositorio);
            var resultadoDto = new PaginacaoResultadoDto<CodafConferenciaResumoDto>(
                mapper.Map<List<CodafConferenciaResumoDto>>(resultado.Itens),
                resultado.TotalRegistros,
                resultado.TamanhoPagina);
            return resultadoDto;
        }
    }
}
