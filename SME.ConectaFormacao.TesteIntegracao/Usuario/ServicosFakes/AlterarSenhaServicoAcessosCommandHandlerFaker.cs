using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.TesteIntegracao.Usuario.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.Usuario.ServicosFakes
{
    internal class AlterarSenhaServicoAcessosCommandHandlerFaker : IRequestHandler<AlterarSenhaServicoAcessosCommand, bool>
    {
        public Task<bool> Handle(AlterarSenhaServicoAcessosCommand request, CancellationToken cancellationToken)
        {
            if (UsuarioAlterarSenhaMock.Login == request.Login && UsuarioAlterarSenhaMock.AlterarSenhaUsuarioDTOValido.SenhaAtual == request.SenhaAtual)
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
