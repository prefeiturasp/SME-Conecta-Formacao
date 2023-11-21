using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Grupo;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterGruposGestaoAcessosQuery : IRequest<IEnumerable<GrupoDTO>>
    {
    }
}
