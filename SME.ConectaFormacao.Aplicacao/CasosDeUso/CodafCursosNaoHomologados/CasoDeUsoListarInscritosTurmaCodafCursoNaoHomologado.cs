using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCursosNaoHomologados
{
    public class CasoDeUsoListarInscritosTurmaCodafCursoNaoHomologado(
        IRepositorioCodafCursoNaoHomologadoInscricao repositorio,
        IMapper mapper) : ICasoDeUsoListarInscritosTurmaCodafCursoNaoHomologado
    {
        public async Task<Resultado<PaginacaoResultadoDto<CodafCursoNaoHomologadoInscritoTurmaDto>>>
            ExecutarAsync(long propostaTurmaId, int numeroPagina = 1, int numeroRegistros = 10)
        {
            var resultado = await repositorio.ObterInscritosPorTurmaAsync(propostaTurmaId, numeroPagina, numeroRegistros);

            var inscritosDto = new PaginacaoResultadoDto<CodafCursoNaoHomologadoInscritoTurmaDto>(
                mapper.Map<List<CodafCursoNaoHomologadoInscritoTurmaDto>>(resultado.Itens),
                resultado.TotalRegistros,
                resultado.TamanhoPagina);
            return inscritosDto;
        }
    }
}
