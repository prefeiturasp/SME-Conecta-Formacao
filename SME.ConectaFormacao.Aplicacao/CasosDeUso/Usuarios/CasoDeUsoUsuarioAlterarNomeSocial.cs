using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.ServicoAcessos.AlterarNomeSocialServicoAcessos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuarios
{
    public class CasoDeUsoUsuarioAlterarNomeSocial(IMediator mediator) : CasoDeUsoAbstrato(mediator), ICasoDeUsoUsuarioAlterarNomeSocial
    {
        public async Task<bool> Executar(string login, string? nome)
        {
            await mediator.Send(new AlterarNomeSocialServicoAcessosCommand(login, nome));
            return true;
        }
    }
}
