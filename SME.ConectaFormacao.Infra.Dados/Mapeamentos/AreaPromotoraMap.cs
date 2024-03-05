using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class AreaPromotoraMap : BaseMapAuditavel<AreaPromotora>
    {
        public AreaPromotoraMap()
        {
            ToTable("area_promotora");

            Map(c => c.Nome).ToColumn("nome");
            Map(c => c.Tipo).ToColumn("tipo");
            Map(c => c.Email).ToColumn("email");
            Map(c => c.GrupoId).ToColumn("grupo_id");
            Map(c => c.DreId).ToColumn("dreid");

            Map(c => c.Dre).Ignore();
            Map(c => c.Telefones).Ignore();
        }
    }
}
