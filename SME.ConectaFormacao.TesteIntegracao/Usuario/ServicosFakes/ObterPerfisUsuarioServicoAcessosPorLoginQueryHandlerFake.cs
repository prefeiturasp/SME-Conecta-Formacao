using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.TesteIntegracao.Usuario.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.Usuario.ServicosFakes
{
    internal class ObterPerfisUsuarioServicoAcessosPorLoginQueryHandlerFake : IRequestHandler<ObterPerfisUsuarioServicoAcessosPorLoginQuery, UsuarioPerfisRetornoDTO>
    {
        public Task<UsuarioPerfisRetornoDTO> Handle(ObterPerfisUsuarioServicoAcessosPorLoginQuery request, CancellationToken cancellationToken)
        {
            if (request.Login == UsuarioRecuperarSenhaMock.LoginValido)
            {
                return Task.FromResult(UsuarioRecuperarSenhaMock.UsuarioPerfisRetornoDTOValido);
            }

            return Task.FromResult(new UsuarioPerfisRetornoDTO());
        }
    }
}
