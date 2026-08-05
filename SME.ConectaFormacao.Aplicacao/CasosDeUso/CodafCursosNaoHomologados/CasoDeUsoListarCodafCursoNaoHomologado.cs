using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCursosNaoHomologados
{
    public class CasoDeUsoListarCodafCursoNaoHomologado(
        IRepositorioCodafCursoNaoHomologado repositorio,
        IMapper mapper) : ICasoDeUsoListarCodafCursoNaoHomologado
    {
        public async Task<Resultado<PaginacaoResultadoDto<CodafCursoNaoHomologadoResumoDto>>> ExecutarAsync(FiltroCodafCursoNaoHomologadoDto filtro)
        {
            var filtroRepositorio = mapper.Map<FiltroListagemResultadoCodafCursoNaoHomologadoDto>(filtro);
            var resultado = await repositorio.ObterListagemResultadoCodafCursoNaoHomologadoPorFiltroAsync(filtroRepositorio);
            var resultadoDto = new PaginacaoResultadoDto<CodafCursoNaoHomologadoResumoDto>(
                mapper.Map<List<CodafCursoNaoHomologadoResumoDto>>(resultado.Itens),
                resultado.TotalRegistros,
                resultado.TamanhoPagina);
            return resultadoDto;
        }
    }
}