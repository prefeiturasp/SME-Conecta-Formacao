using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaCriterioCertificacaoMap : BaseMapAuditavel<PropostaCriterioCertificacao>
    {
        public PropostaCriterioCertificacaoMap()
        {
            ToTable("proposta_criterio_certificacao");
            Map(t => t.PropostaId).ToColumn("proposta_id");
            Map(t => t.CriterioCertificacaoId).ToColumn("criterio_certificacao_id");
        }
    }
}