using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAreaPromotoraPorGrupoIdEDresQuery : IRequest<Dominio.Entidades.AreaPromotora>
    {
        public ObterAreaPromotoraPorGrupoIdEDresQuery(Guid grupoId, string[] dres)
        {
            GrupoId = grupoId;
            Dres = dres;
        }

        public Guid GrupoId { get; }
        public string[] Dres { get; set; }
    }
}
