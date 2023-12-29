using MediatR;
using SME.ConectaFormacao.Aplicacao.Consultas;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterPropostaRegentePaginacao : CasoDeUsoAbstratoPaginado, ICasoDeUsoObterPropostaRegentePaginacao
    {
        public CasoDeUsoObterPropostaRegentePaginacao(IMediator mediator, IContextoAplicacao contextoAplicacao) : base(mediator, contextoAplicacao)
        {
        }

        public async Task<PaginacaoResultadoDTO<PropostaRegenteDTO>> Executar(long id)
        {
            if (id == 0) return new PaginacaoResultadoDTO<PropostaRegenteDTO>(new List<PropostaRegenteDTO>(), 0, 0);
            return await mediator.Send(new ObterRegentesPaginadoQuery(id, NumeroPagina, NumeroRegistros));
        }
    }
}