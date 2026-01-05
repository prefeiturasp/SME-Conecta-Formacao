using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuarios
{
    public class CasoDeUsoUsuarioValidacaoSenhaToken : CasoDeUsoAbstrato, ICasoDeUsoUsuarioValidacaoSenhaToken
    {
        public CasoDeUsoUsuarioValidacaoSenhaToken(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(Guid token)
        {
            return await mediator.Send(new ValidarUsuarioTokenServicoAcessosQuery(token));
        }
    }
}
