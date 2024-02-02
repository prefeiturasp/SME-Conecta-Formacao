using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuario
{
    public class CasoDeUsoUsuarioValidacaoToken : CasoDeUsoAbstrato, ICasoDeUsoUsuarioValidacaoToken
    {
        public CasoDeUsoUsuarioValidacaoToken(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(Guid token)
        {
            return await mediator.Send(new ValidarUsuarioTokenServicoAcessosQuery(token));
        }
    }
}
