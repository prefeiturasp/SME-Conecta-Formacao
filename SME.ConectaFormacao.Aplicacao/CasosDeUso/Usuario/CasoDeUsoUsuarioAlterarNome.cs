using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuario
{
    public class CasoDeUsoUsuarioAlterarNome : CasoDeUsoAbstrato, ICasoDeUsoUsuarioAlterarNome
    {
        public CasoDeUsoUsuarioAlterarNome(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(string login, string nome)
        {
            await mediator.Send(new AlterarNomeServicoAcessosCommand(login, nome));
            
            return await mediator.Send(new SalvarUsuarioParcialCommand(login, nome));
        }
    }
}
