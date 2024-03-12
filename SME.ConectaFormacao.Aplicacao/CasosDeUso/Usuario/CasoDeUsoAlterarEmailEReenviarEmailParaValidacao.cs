using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuario
{
    public class CasoDeUsoAlterarEmailEReenviarEmailParaValidacao : CasoDeUsoAbstrato, ICasoDeUsoAlterarEmailEReenviarEmailParaValidacao
    {
        public CasoDeUsoAlterarEmailEReenviarEmailParaValidacao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(string login, string email)
        {
            await mediator.Send(new AlterarEmailServicoAcessosCommand(login, email));
            return await mediator.Send(new EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand(login));
        }
    }
}