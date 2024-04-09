using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Funcionario;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuariosAdminDfQuery : IRequest<IEnumerable<UsuarioAdminDfDTO>>
    {
    }
}