using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaComponenteCurricularMap : BaseMapAuditavel<PropostaComponenteCurricular>
    {
        public PropostaComponenteCurricularMap()
        {
            ToTable("proposta_componente_curricular");

            Map(t => t.PropostaId).ToColumn("proposta_id");
            Map(t => t.ComponenteCurricularId).ToColumn("componente_curricular_id");
        }
    }
}
