using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoListarCodafListaPresenca(
        IRepositorioCodafListaPresenca repositorioCodafListaPresenca,
        IMapper mapper) : ICasoDeUsoListarCodafListaPresenca
    {
        public async Task<Resultado<PaginacaoResultadoDto<ListaPresencaCodafResumoDto>>> ExecutarAsync(FiltroListaPresencaCodafDto filtro)
        {
            var filtroRepositorio = mapper.Map<FiltroListagemResultadoCodafListaPresencaDto>(filtro);
            var resultado = await repositorioCodafListaPresenca.ObterListagemResultadoCodafListaPresencaPorFiltroAsync(filtroRepositorio);
            var resultadoDto = mapper.Map<PaginacaoResultadoDto<ListaPresencaCodafResumoDto>>(resultado);
            return resultadoDto;
        }
    }
}
