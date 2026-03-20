using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterPropostaEncontroPaginacao : CasoDeUsoAbstratoPaginado, ICasoDeUsoObterPropostaEncontroPaginacao
    {
        public CasoDeUsoObterPropostaEncontroPaginacao(IMediator mediator, IContextoAplicacao contextoAplicacao) : base(mediator, contextoAplicacao)
        {
        }

        public async Task<PaginacaoResultadoDto<PropostaEncontroDto>> Executar(long id)
        {
            if (id == 0) return new PaginacaoResultadoDto<PropostaEncontroDto>(new List<PropostaEncontroDto>(), 0, 0);

            return await mediator.Send(new ObterEncontrosPaginadoQuery(id, NumeroPagina, NumeroRegistros));
        }
    }
}
