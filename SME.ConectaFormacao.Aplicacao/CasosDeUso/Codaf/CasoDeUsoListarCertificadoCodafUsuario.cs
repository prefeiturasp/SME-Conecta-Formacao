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
        public async Task<Resultado<PaginacaoResultadoDto<ListagemResultadoCertificadoCodafUsuarioDto>>> ExecutarAsync(FiltroListaCertificadoCodafDto filtro)
        {
            var filtroRepositorio = mapper.Map<FiltroListagemResultadoCertificadoCodafUsuarioDto>(filtro);
            var resultado = await repositorioCodafCertificado.ObterListagemCertificadoDoUsuarioPorFiltroAsync(filtroRepositorio);
            var resultadoDto = new PaginacaoResultadoDto<ListagemResultadoCertificadoCodafUsuarioDto>(
                resultado.Itens,
                resultado.TotalRegistros,
                resultado.TamanhoPagina);
            return resultadoDto;
        }
    }
}