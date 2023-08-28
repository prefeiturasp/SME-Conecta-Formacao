using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.ServicosFakes
{
    public class ValidarTokenRecuperacaoSenhaServicoAcessosQueryHandlerFake : IRequestHandler<ValidarTokenRecuperacaoSenhaServicoAcessosQuery, bool>
    {
        public Task<bool> Handle(ValidarTokenRecuperacaoSenhaServicoAcessosQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(request.Token == UsuarioRecuperarSenhaMock.TokenValido);
        }
    }
}
