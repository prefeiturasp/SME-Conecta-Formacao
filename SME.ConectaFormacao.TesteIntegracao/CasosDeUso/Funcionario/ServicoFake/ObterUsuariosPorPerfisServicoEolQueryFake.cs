using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Funcionario
{
    public class ObterUsuariosPorPerfisServicoEolQueryFake : IRequestHandler<ObterUsuariosPorPerfisServicoEolQuery, IEnumerable<UsuarioPerfilServicoEol>>
    {
        public async Task<IEnumerable<UsuarioPerfilServicoEol>> Handle(ObterUsuariosPorPerfisServicoEolQuery request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(UsuarioPerfilServicoEolMock.GerarListaUsuariosPerfis());
        }
    }
}