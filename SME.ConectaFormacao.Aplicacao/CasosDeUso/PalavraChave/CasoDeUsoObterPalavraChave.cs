using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.PalavraChave;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.PalavraChave
{
    public class CasoDeUsoObterPalavraChave : CasoDeUsoAbstrato, ICasoDeUsoObterPalavraChave
    {
        public CasoDeUsoObterPalavraChave(IMediator mediator) : base(mediator)
        {}
        public async Task<IEnumerable<RetornoListagemDTO>> Executar()
        {
            return await mediator.Send(ObterPalavraChaveQuery.Instance);
        }
    }
}
