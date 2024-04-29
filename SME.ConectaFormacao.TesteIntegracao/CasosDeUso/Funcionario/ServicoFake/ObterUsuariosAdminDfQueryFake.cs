using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Funcionario
{
    public class ObterUsuariosAdminDfQueryFake : IRequestHandler<ObterUsuariosAdminDfQuery, IEnumerable<RetornoUsuarioLoginNomeDTO>>
    {
        public async Task<IEnumerable<RetornoUsuarioLoginNomeDTO>> Handle(ObterUsuariosAdminDfQuery request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(UsuarioAdminDfMock.GerarListaUsuariosAdminDf());
        }
    }
}