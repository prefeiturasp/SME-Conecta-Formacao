using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaPalavraChaveMap : BaseMapAuditavel<PropostaPalavraChave>
    {
        public PropostaPalavraChaveMap()
        {
            ToTable("proposta_palavra_chave");

            Map(c => c.PropostaId).ToColumn("proposta_id");
            Map(c => c.PalavraChaveId).ToColumn("palavra_chave_id");
        }
    }
}
