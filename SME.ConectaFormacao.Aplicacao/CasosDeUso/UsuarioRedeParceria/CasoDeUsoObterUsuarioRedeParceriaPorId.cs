using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.UsuarioRedeParceria;
using SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.UsuarioRedeParceria
{
    public class CasoDeUsoObterUsuarioRedeParceriaPorId : CasoDeUsoAbstrato, ICasoDeUsoObterUsuarioRedeParceriaPorId
    {
        public CasoDeUsoObterUsuarioRedeParceriaPorId(IMediator mediator) : base(mediator)
        {
        }

        public async Task<UsuarioRedeParceriaDTO> Executar(long id)
        {
            return await mediator.Send(new ObterUsuarioRedeParceriaPorIdQuery(id));
        }
    }
}
