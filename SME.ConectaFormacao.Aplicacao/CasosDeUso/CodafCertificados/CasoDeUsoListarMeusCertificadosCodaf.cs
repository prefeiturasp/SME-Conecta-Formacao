using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafCertificados;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCertificados
{
    public class CasoDeUsoListarMeusCertificadosCodaf(
        IRepositorioCodafCertificado repositorioCodafCertificado,
        IMapper mapper) : ICasoDeUsoListarMeusCertificadosCodaf
    {
        public async Task<Resultado<PaginacaoResultadoDto<MeusCertificadosCodafDto>>> ExecutarAsync(FiltroListaMeusCertificadosCodafDto filtro)
        {
            var filtroRepositorio = mapper.Map<FiltroMeusCertificadosCodafDto>(filtro);
            var resultado = await repositorioCodafCertificado.ObterMeusCertificadosPorFiltroAsync(filtroRepositorio);
            var resultadoDto = new PaginacaoResultadoDto<MeusCertificadosCodafDto>(
                resultado.Itens,
                resultado.TotalRegistros,
                resultado.TamanhoPagina);
            return resultadoDto;
        }
    }
}