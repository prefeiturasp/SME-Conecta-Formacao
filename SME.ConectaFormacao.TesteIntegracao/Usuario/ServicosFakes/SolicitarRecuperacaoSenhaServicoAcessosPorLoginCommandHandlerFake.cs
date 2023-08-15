using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.TesteIntegracao.Usuario.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.Usuario.ServicosFakes
{
    public class SolicitarRecuperacaoSenhaServicoAcessosPorLoginCommandHandlerFake : IRequestHandler<SolicitarRecuperacaoSenhaServicoAcessosPorLoginCommand, string>
    {
        public Task<string> Handle(SolicitarRecuperacaoSenhaServicoAcessosPorLoginCommand request, CancellationToken cancellationToken)
        {
            if (request.Login == UsuarioRecuperarSenhaMock.LoginValido)
            {
                return Task.FromResult(UsuarioRecuperarSenhaMock.EmailValido);
            }

            return Task.FromResult(string.Empty);
        }
    }
}
