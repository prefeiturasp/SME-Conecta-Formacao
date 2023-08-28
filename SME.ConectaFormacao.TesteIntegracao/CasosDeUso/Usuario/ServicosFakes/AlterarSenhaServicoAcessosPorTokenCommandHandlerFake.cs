using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.ServicosFakes
{
    public class AlterarSenhaServicoAcessosPorTokenCommandHandlerFake : IRequestHandler<AlterarSenhaServicoAcessosPorTokenCommand, string>
    {
        public Task<string> Handle(AlterarSenhaServicoAcessosPorTokenCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(UsuarioRecuperarSenhaMock.LoginValido);
        }
    }
}
