using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class AreaPromotoraTelefoneMap : BaseMapAuditavel<AreaPromotoraTelefone>
    {
        public AreaPromotoraTelefoneMap()
        {
            ToTable("area_promotora_telefone");
            Map(c => c.AreaPromotoraId).ToColumn("area_promotora_id");
            Map(c => c.Telefone).ToColumn("telefone");
        }
    }
}
