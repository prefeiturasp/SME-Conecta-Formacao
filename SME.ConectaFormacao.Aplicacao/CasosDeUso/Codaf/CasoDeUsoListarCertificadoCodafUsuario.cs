using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoListarCertificadoCodafUsuario(
        IRepositorioCodafCertificado repositorioCodafCertificado,
        IMapper mapper) : ICasoDeUsoListarCertificadoCodafUsuario
    {
        public async Task<Resultado<PaginacaoResultadoDto<ListagemResultadoCertificadoCodafDto>>> ExecutarAsync(FiltroListaCertificadoCodafDto filtro)
        {
            var filtroRepositorio = mapper.Map<FiltroListagemResultadoCertificadoCodafDto>(filtro);
            var resultado = await repositorioCodafCertificado.ObterListagemCertificadoPorFiltroAsync(filtroRepositorio);
            var resultadoDto = new PaginacaoResultadoDto<ListagemResultadoCertificadoCodafDto>(
                resultado.Itens,
                resultado.TotalRegistros,
                resultado.TamanhoPagina);
            return resultadoDto;
        }
    }
}