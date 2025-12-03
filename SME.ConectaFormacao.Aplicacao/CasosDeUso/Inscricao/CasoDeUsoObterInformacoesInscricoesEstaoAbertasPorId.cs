using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoObterInformacoesInscricoesEstaoAbertasPorId(IMediator mediator) : 
        CasoDeUsoAbstrato(mediator), ICasoDeUsoObterInformacoesInscricoesEstaoAbertasPorId
    {
        public async Task<PodeInscreverMensagemDTO> Executar(long propostaId)
        {
            return await mediator.Send(new ObterInformacoesInscricoesEstaoAbertasPorIdQuery(propostaId));
        }
    }
}