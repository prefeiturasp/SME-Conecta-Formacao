using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaPareceristaConsideracaoMap : BaseMapAuditavel<PropostaPareceristaConsideracao>
    {
        public PropostaPareceristaConsideracaoMap()
        {
            ToTable("proposta_parecerista_consideracao");

            Map(t => t.PropostaPareceristaId).ToColumn("proposta_parecerista_id");
            Map(t => t.Campo).ToColumn("campo");
            Map(t => t.Descricao).ToColumn("descricao");
        }
    }
}
