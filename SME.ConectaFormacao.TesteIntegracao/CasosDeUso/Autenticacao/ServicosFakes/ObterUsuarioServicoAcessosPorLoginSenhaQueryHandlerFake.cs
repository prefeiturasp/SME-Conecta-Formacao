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
                var usuarioAUtenticacao = ObterUsuarioAutenticacao();
                usuarioAUtenticacao.Login = request.Login;
                return Task.FromResult(usuarioAUtenticacao);
            }

            return Task.FromResult(ObterUsuarioAutenticacao());
        }

        private static UsuarioAutenticacaoRetornoDTO ObterUsuarioAutenticacao()
        {
            return new UsuarioAutenticacaoRetornoDTO
            {
                Login = AutenticacaoMock.UsuarioLogado.Login,
                Nome = AutenticacaoMock.UsuarioLogado.Nome,
                Email = AutenticacaoMock.UsuarioLogado.Email
            };
        }
    }
}
