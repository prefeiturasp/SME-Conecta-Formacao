using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Grupo;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterGruposGestaoQuery : IRequest<IEnumerable<GrupoDTO>>
    {
    }
}
