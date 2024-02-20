using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Grupo;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterGruposServicoAcessosQuery : IRequest<IEnumerable<GrupoDTO>>
    {
    }
}
