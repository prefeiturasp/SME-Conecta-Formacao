using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPerfilAreaPromotoraQuery : IRequest<RetornoListagemDTO>
    {
        public ObterPerfilAreaPromotoraQuery(Guid grupoId)
        {
            GrupoId = grupoId;
        }

        public Guid GrupoId { get; }
    }
}
