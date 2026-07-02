using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoListarCodafListaPresenca(
        IRepositorioCodafListaPresenca repositorioCodafListaPresenca,
        IMapper mapper, 
        IValidadorPermissaoCodaf validadorPermissaoCodaf, 
        IMediator _mediator) : ICasoDeUsoListarCodafListaPresenca
    {
        public async Task<Resultado<PaginacaoResultadoDto<ListaPresencaCodafResumoDto>>> ExecutarAsync(FiltroListaPresencaCodafDto filtro)
        {
            var filtroRepositorio = mapper.Map<FiltroListagemResultadoCodafListaPresencaDto>(filtro);
            var resultado = await repositorioCodafListaPresenca.ObterListagemResultadoCodafListaPresencaPorFiltroAsync(filtroRepositorio);

            var usuarioLogado = await _mediator.Send(new ObterUsuarioLogadoQuery()) ?? throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO, System.Net.HttpStatusCode.Unauthorized);
            var perfilUsuario = await validadorPermissaoCodaf.BuscarPerfilUsuario();

            if (await validadorPermissaoCodaf.UsuarioPossuiPerfilAdminOuEMFORPEF(perfilUsuario))
            {
                var paginacaoResultadoDto = new PaginacaoResultadoDto<ListaPresencaCodafResumoDto>(
                    mapper.Map<IEnumerable<ListaPresencaCodafResumoDto>>(resultado.Itens),
                    resultado.TotalRegistros,
                    resultado.TamanhoPagina); 

                return paginacaoResultadoDto;
            }

            var itensAutorizados = new List<ListaPresencaCodafResumoDto>();

            foreach (var item in resultado.Itens)
            {
                if (await validadorPermissaoCodaf.ValidarSeUsuarioEhCriador(usuarioLogado, item.Id))
                {
                    itensAutorizados.Add(mapper.Map<ListaPresencaCodafResumoDto>(item));
                }
            }

            var paginacaoResultadoDtoFinal = new PaginacaoResultadoDto<ListaPresencaCodafResumoDto>(
                itensAutorizados,
                itensAutorizados.Count,
                resultado.TamanhoPagina);

            return paginacaoResultadoDtoFinal;
        }
    }
}
