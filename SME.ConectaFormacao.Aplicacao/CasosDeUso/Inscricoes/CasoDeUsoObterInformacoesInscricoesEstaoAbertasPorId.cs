using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes
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