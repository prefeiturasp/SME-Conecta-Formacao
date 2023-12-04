using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaDreMap : BaseMapAuditavel<PropostaDre>
    {
        public PropostaDreMap()
        {
            ToTable("proposta_dre");
            Map(c => c.PropostaId).ToColumn("proposta_id");
            Map(c => c.DreId).ToColumn("dre_id");
        }
    }
}
