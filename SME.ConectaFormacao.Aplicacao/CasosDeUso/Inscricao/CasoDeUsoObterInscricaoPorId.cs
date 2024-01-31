using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoObterInscricaoPorId : CasoDeUsoAbstratoPaginado, ICasoDeUsoObterInscricaoPorId
    {
        public CasoDeUsoObterInscricaoPorId(IMediator mediator, IContextoAplicacao contextoAplicacao) : base(mediator, contextoAplicacao)
        {
        }

        public async Task<PaginacaoResultadoDTO<DadosListagemInscricaoDTO>> Executar(long propostaId, FiltroListagemInscricaoDTO filtroListagemInscricaoDTO)
        {
            return await mediator.Send(new ObterInscricaoPorIdQuery(propostaId, filtroListagemInscricaoDTO, NumeroPagina, NumeroRegistros));
        }
    }
}