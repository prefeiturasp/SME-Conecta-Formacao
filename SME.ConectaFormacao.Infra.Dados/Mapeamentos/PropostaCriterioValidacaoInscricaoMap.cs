using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaCriterioValidacaoInscricaoMap : BaseMapAuditavel<PropostaCriterioValidacaoInscricao>
    {
        public PropostaCriterioValidacaoInscricaoMap()
        {
            ToTable("proposta_criterio_validacao_inscricao");

            Map(m => m.PropostaId).ToColumn("proposta_id");
            Map(m => m.CriterioValidacaoInscricaoId).ToColumn("criterio_validacao_inscricao_id");
        }
    }
}
