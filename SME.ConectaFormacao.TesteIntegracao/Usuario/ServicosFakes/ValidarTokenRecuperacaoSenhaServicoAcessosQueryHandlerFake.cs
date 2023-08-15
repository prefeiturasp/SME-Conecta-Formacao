using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.TesteIntegracao.Usuario.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.Usuario.ServicosFakes
{
    public class ValidarTokenRecuperacaoSenhaServicoAcessosQueryHandlerFake : IRequestHandler<ValidarTokenRecuperacaoSenhaServicoAcessosQuery, bool>
    {
        public Task<bool> Handle(ValidarTokenRecuperacaoSenhaServicoAcessosQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(request.Token == UsuarioRecuperarSenhaMock.TokenValido);
        }
    }
}
