using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuario
{
    public class CasoDeUsoReenviarEmail : CasoDeUsoAbstrato, ICasoDeUsoReenviarEmail
    {
        public CasoDeUsoReenviarEmail(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(string login)
        {
            return await mediator.Send(new EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand(login));
        }
    }
}
