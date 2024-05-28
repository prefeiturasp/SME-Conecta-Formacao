using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Autenticacao;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Autenticacao.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Autenticacao.ServicosFakes
{
    internal class ObterUsuarioServicoAcessosPorLoginSenhaQueryHandlerFake : IRequestHandler<ObterUsuarioServicoAcessosPorLoginSenhaQuery, UsuarioAutenticacaoRetornoDTO>
    {
        public Task<UsuarioAutenticacaoRetornoDTO> Handle(ObterUsuarioServicoAcessosPorLoginSenhaQuery request, CancellationToken cancellationToken)
        {
            var usuario = AutenticacaoMock.AutenticacaoUsuarioDTOValido;
            
            if (request.Login == usuario.Login && usuario.Senha == request.Senha)
            {
                var usuarioAUtenticacao = new UsuarioAutenticacaoRetornoDTO
                {
                    Login = request.Login
                };
                return Task.FromResult(usuarioAUtenticacao);
            }

            return Task.FromResult(new UsuarioAutenticacaoRetornoDTO());
        }
    }
}
