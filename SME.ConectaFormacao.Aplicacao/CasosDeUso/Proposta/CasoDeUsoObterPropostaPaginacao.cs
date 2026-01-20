using MediatR;
using SME.ConectaFormacao.Aplicacao.Consultas.Proposta.ObterPropostaPaginada;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterPropostaPaginacao(IMediator mediator, IContextoAplicacao contextoAplicacao) : 
        CasoDeUsoAbstratoPaginado(mediator, contextoAplicacao), ICasoDeUsoObterPropostaPaginacao
    {
        public async Task<PaginacaoResultadoDto<PropostaPaginadaDTO>> Executar(PropostaFiltrosDTO propostaFiltrosDTO)
        {
            var areaPromotoraUsuarioLogado = await mediator.Send(new ObterAreaPromotoraUsuarioLogadoQuery());
            return await mediator.Send(new ObterPropostaPaginadaQuery(propostaFiltrosDTO, NumeroPagina, NumeroRegistros, areaPromotoraUsuarioLogado?.Id));
        }
    }
}
