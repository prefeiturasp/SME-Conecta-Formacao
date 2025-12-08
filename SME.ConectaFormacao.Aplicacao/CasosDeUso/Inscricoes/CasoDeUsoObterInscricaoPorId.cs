using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes
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