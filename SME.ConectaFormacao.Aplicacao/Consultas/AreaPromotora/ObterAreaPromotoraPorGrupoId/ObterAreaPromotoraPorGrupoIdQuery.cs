using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAreaPromotoraPorGrupoIdQuery : IRequest<Dominio.Entidades.AreaPromotora>
    {
        public ObterAreaPromotoraPorGrupoIdQuery(Guid grupoId, IEnumerable<string> dresCodigo)
        {
            GrupoId = grupoId;
            DresCodigo = dresCodigo;
        }

        public IEnumerable<string> DresCodigo { get; set; }
        public Guid GrupoId { get; }
    }
}
