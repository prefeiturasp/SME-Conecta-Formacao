using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class ImportacaoArquivoMap : BaseMapAuditavel<ImportacaoArquivo>
    {
        public ImportacaoArquivoMap()
        {
            ToTable("importacao_arquivo");
            Map(c => c.PropostaId).ToColumn("proposta_id");
            Map(c => c.Nome).ToColumn("nome");
            Map(c => c.Tipo).ToColumn("tipo");
            Map(c => c.Situacao).ToColumn("situacao");
        }
    }
}
