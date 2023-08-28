using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class RoteiroPropostaFormativaMap : BaseMapAuditavel<RoteiroPropostaFormativa>
    {
        public RoteiroPropostaFormativaMap()
        {
            ToTable("roteiro_proposta_formativa");
            Map(c => c.Descricao).ToColumn("descricao");
        }
    }
}
