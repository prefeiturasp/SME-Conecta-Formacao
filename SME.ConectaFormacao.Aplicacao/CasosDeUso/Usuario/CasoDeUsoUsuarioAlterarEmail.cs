using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuario
{
    public class CasoDeUsoUsuarioAlterarEmail : CasoDeUsoAbstrato, ICasoDeUsoUsuarioAlterarEmail
    {
        public CasoDeUsoUsuarioAlterarEmail(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(string login, string email)
        {
            return await mediator.Send(new AlterarEmailServicoAcessosCommand(login, email));
        }
    }
}
