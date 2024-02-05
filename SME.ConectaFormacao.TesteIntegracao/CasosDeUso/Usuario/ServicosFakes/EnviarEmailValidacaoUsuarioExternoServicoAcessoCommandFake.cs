using MediatR;
using SME.ConectaFormacao.Aplicacao;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.ServicosFakes
{
    public class EnviarEmailValidacaoUsuarioExternoServicoAcessoCommandFake : IRequestHandler<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand, bool>
    {
        public async Task<bool> Handle(EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand request, CancellationToken cancellationToken)
        {
           return true;
        }
    }
}
