using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaModalidadeAnoTurmaEtapaMap : BaseMapAuditavel<PropostaModalidadeAnoTurmaEtapa>
    {
        public PropostaModalidadeAnoTurmaEtapaMap()
        {
            ToTable("proposta_modalidade_ano_turma_etapa");

            Map(t => t.Modalidade).ToColumn("modalidade");
            Map(t => t.AnoTurmaId).ToColumn("ano_turma_id");
            Map(t => t.EtapaEja).ToColumn("etapa_eja");
        }
    }
}
