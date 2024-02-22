using MediatR;
using SME.ConectaFormacao.Aplicacao.CasosDeUso;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class CasoDeUsoObterInscricaoTipo :CasoDeUsoAbstrato,ICasoDeUsoObterInscricaoTipo
    {
        public CasoDeUsoObterInscricaoTipo(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<RetornoListagemDTO>> Executar()
        {
            return await mediator.Send(new ObterInscricaoTipoListaQuery());
        }
    }
}