using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoListarInscritosTurmaCodafListaPresenca(
        IRepositorioCodafInscritosListaPresenca repositorioCodafInscritosListaPresenca,
        IMapper mapper) : ICasoDeUsoListarInscritosTurmaCodafListaPresenca
    {
        public async Task<Resultado<PaginacaoResultadoDto<CodafInscritoTurmaListaPresencaRetornoDto>>> ExecutarAsync(long propostaTurmaId, int numeroPagina = 1, int numeroRegistros = 10)
        {
            var resultado = await repositorioCodafInscritosListaPresenca.ObterInscritosPorTurmaAsync(propostaTurmaId, numeroPagina, numeroRegistros);

            var inscritosDto = new PaginacaoResultadoDto<CodafInscritoTurmaListaPresencaRetornoDto>(
                mapper.Map<List<CodafInscritoTurmaListaPresencaRetornoDto>>(resultado.Itens),
                resultado.TotalRegistros,
                resultado.TamanhoPagina);
            return inscritosDto;
        }
    }
}