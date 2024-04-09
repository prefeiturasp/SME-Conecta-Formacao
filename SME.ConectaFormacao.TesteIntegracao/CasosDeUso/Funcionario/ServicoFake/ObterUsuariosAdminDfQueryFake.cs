using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Funcionario;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Funcionario
{
    public class ObterUsuariosAdminDfQueryFake : IRequestHandler<ObterUsuariosAdminDfQuery, IEnumerable<UsuarioAdminDfDTO>>
    {
        public async Task<IEnumerable<UsuarioAdminDfDTO>> Handle(ObterUsuariosAdminDfQuery request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(UsuarioAdminDfMock.GerarListaUsuariosAdminDf());
        }
    }
}