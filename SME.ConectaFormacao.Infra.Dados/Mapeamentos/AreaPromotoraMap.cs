using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class AreaPromotoraMap : BaseMapAuditavel<AreaPromotora>
    {
        public AreaPromotoraMap()
        {
            ToTable("area_promotora");
            Map(c => c.Nome).ToColumn("nome");
            Map(c => c.Email).ToColumn("email");
            Map(c => c.PerfilId).ToColumn("perfil_id");
        }
    }
}
