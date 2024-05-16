using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.UsuarioRedeParceria
{
    public class CasoDeUsoObterSituacaoUsuarioRedeParceria : CasoDeUsoAbstrato, ICasoDeUsoObterSituacaoUsuarioRedeParceria
    {
        public CasoDeUsoObterSituacaoUsuarioRedeParceria(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<RetornoListagemDTO>> Executar()
        {
            return await mediator.Send(ObterSituacaoUsuarioRedeParceriaQuery.Instancia());
        }
    }
}
