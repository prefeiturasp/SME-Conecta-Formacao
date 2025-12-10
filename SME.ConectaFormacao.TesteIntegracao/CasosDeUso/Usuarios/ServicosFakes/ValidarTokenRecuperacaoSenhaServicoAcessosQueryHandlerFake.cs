using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuarios.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuarios.ServicosFakes
{
    public class ValidarTokenRecuperacaoSenhaServicoAcessosQueryHandlerFake : IRequestHandler<ValidarUsuarioTokenServicoAcessosQuery, bool>
    {
        public Task<bool> Handle(ValidarUsuarioTokenServicoAcessosQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(request.Token == UsuarioRecuperarSenhaMock.TokenValido);
        }
    }
}
