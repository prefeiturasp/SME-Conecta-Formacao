using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class ImportacaoArquivoRegistroMap : BaseMapAuditavel<ImportacaoArquivoRegistro>
    {
        public ImportacaoArquivoRegistroMap()
        {
            ToTable("importacao_arquivo_registro");
            Map(c => c.ImportacaoArquivoId).ToColumn("importacao_arquivo_id");
            Map(c => c.Linha).ToColumn("linha");
            Map(c => c.Conteudo).ToColumn("conteudo");
            Map(c => c.Erro).ToColumn("erro");
            Map(c => c.Situacao).ToColumn("situacao");
        }
    }
}
