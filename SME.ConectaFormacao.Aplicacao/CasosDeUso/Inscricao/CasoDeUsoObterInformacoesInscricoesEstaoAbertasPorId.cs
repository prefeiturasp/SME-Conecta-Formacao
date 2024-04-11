using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoObterInformacoesInscricoesEstaoAbertasPorId : CasoDeUsoAbstrato, ICasoDeUsoObterInformacoesInscricoesEstaoAbertasPorId
    {
        public CasoDeUsoObterInformacoesInscricoesEstaoAbertasPorId(IMediator mediator) : base(mediator)
        {
        }

        public async Task<PodeInscreverMensagemDTO> Executar(long id)
        {
            return await mediator.Send(new ObterInformacoesInscricoesEstaoAbertasPorIdQuery(id));
        }
    }
}