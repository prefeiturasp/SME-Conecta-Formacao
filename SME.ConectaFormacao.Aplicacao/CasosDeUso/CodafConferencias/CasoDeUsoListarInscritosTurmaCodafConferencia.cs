using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafConferencias;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafConferencias;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafConferencias
{
    public class CasoDeUsoListarInscritosTurmaCodafConferencia(
        IRepositorioCodafConferenciaInscricao repositorio,
        IMapper mapper) : ICasoDeUsoListarInscritosTurmaCodafConferencia
    {
        public async Task<Resultado<PaginacaoResultadoDto<CodafConferenciaInscritoTurmaDto>>> ExecutarAsync(
            long propostaTurmaId,
            int numeroPagina = 1,
            int numeroRegistros = 10,
            long? codafConferenciaId = null)
        {
            var resultado = await repositorio.ObterInscritosPorTurmaAsync(propostaTurmaId, numeroPagina, numeroRegistros, codafConferenciaId);

            var inscritosDto = new PaginacaoResultadoDto<CodafConferenciaInscritoTurmaDto>(
                mapper.Map<List<CodafConferenciaInscritoTurmaDto>>(resultado.Itens),
                resultado.TotalRegistros,
                resultado.TamanhoPagina);
            return inscritosDto;
        }
    }
}
