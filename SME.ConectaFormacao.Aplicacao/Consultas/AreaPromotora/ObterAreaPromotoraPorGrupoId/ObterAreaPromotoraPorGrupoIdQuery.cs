using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAreaPromotoraPorGrupoIdQuery : IRequest<Dominio.Entidades.AreaPromotora>
    {
        public ObterAreaPromotoraPorGrupoIdQuery(Guid grupoId)
        {
            GrupoId = grupoId;
        }

        public Guid GrupoId { get; }
    }
}
