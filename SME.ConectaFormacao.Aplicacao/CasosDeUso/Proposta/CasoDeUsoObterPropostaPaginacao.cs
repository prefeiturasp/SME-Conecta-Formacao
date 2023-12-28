using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterPropostaPaginacao : CasoDeUsoAbstratoPaginado, ICasoDeUsoObterPropostaPaginacao
    {
        public CasoDeUsoObterPropostaPaginacao(IMediator mediator, IContextoAplicacao contextoAplicacao) : base(mediator, contextoAplicacao)
        {
        }

        public async Task<PaginacaoResultadoDTO<PropostaPaginadaDTO>> Executar(PropostaFiltrosDTO propostaFiltrosDTO)
        {
            return await mediator.Send(new ObterPropostaPaginadaQuery(propostaFiltrosDTO, NumeroPagina, NumeroRegistros));
        }
    }
}
