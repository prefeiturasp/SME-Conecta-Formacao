using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafCertificados;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCertificados
{
    public class CasoDeUsoListarTodosCertificadosCodaf(
        IRepositorioCodafCertificado repositorioCodafCertificado,
        IMapper mapper) : ICasoDeUsoListarTodosCertificadosCodaf
    {
        public async Task<Resultado<PaginacaoResultadoDto<ListagemCertificadosCodafDto>>> ExecutarAsync(FiltroListaTodosCertificadosCodafDto filtro)
        {
            var filtroRepositorio = mapper.Map<FiltroListagemTodosCertificadosCodafDto>(filtro);
            var resultado = await repositorioCodafCertificado.ObterTodosCertificadosAsync(filtroRepositorio);
            var resultadoDto = new PaginacaoResultadoDto<ListagemCertificadosCodafDto>(
                resultado.Itens,
                resultado.TotalRegistros,
                resultado.TamanhoPagina);
            return resultadoDto;
        }
    }
}