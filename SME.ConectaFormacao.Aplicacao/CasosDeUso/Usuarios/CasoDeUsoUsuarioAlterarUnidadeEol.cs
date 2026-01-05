using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuarios
{
    public class CasoDeUsoUsuarioAlterarUnidadeEol : CasoDeUsoAbstrato, ICasoDeUsoUsuarioAlterarUnidadeEol
    {
        public CasoDeUsoUsuarioAlterarUnidadeEol(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(string login, string codigoEolUnidade)
        {
            return await mediator.Send(new AlterarUnidadeEolUsuarioCommand(login, codigoEolUnidade));
        }
    }
}
