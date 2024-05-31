using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Autenticacao.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Autenticacao.ServicosFakes
{
    internal class ObterPerfisUsuarioServicoAcessosPorLoginQueryHandlerFake : IRequestHandler<ObterPerfisUsuarioServicoAcessosPorLoginQuery, UsuarioPerfisRetornoDTO>
    {
        public Task<UsuarioPerfisRetornoDTO> Handle(ObterPerfisUsuarioServicoAcessosPorLoginQuery request, CancellationToken cancellationToken)
        {
            if (request.Login == AutenticacaoMock.AutenticacaoUsuarioDTOValido.Login)
            {
                var usuario = AutenticacaoMock.UsuarioPerfisRetornoDTOValido;
                usuario.UsuarioLogin = request.Login;
                return Task.FromResult(usuario);
            }

            return Task.FromResult(new UsuarioPerfisRetornoDTO
            {
                UsuarioLogin = AutenticacaoMock.UsuarioLogado.Login,
                UsuarioNome = AutenticacaoMock.UsuarioLogado.Nome,
                Email = AutenticacaoMock.UsuarioLogado.Email
            });
        }
    }
}
