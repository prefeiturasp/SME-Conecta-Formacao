using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoListarCodafListaPresenca(
        IRepositorioCodafListaPresenca repositorioCodafListaPresenca,
        IContextoAplicacao contextoAplicacao,
        IMapper mapper) : ICasoDeUsoListarCodafListaPresenca
    {
        public async Task<Resultado<PaginacaoResultadoDto<ListaPresencaCodafResumoDto>>> ExecutarAsync(FiltroListaPresencaCodafDto filtro)
        {
            var filtroRepositorio = mapper.Map<FiltroListagemResultadoCodafListaPresencaDto>(filtro);

            filtroRepositorio.PerfilRestrito = contextoAplicacao.IdPerfilUsuario != Perfis.ADMIN_DF && contextoAplicacao.IdPerfilUsuario != Perfis.EMFORPEF;

            var resultado = await repositorioCodafListaPresenca.ObterListagemResultadoCodafListaPresencaPorFiltroAsync(filtroRepositorio);

            var resultadoDto = new PaginacaoResultadoDto<ListaPresencaCodafResumoDto>(
                mapper.Map<List<ListaPresencaCodafResumoDto>>(resultado.Itens),
                resultado.TotalRegistros,
                resultado.TamanhoPagina);
            return resultadoDto;
        }
    }
}