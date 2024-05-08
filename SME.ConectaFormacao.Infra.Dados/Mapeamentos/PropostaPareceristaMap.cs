using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaPareceristaMap : BaseMapAuditavel<PropostaParecerista>
    {
        public PropostaPareceristaMap()
        {
            ToTable("proposta_parecerista");
            Map(t => t.PropostaId).ToColumn("proposta_id");
            Map(t => t.RegistroFuncional).ToColumn("registro_funcional");
            Map(t => t.NomeParecerista).ToColumn("nome_parecerista");
            Map(t => t.Situacao).ToColumn("situacao");
            Map(t => t.Justificativa).ToColumn("justificativa");
        }
    }
}
