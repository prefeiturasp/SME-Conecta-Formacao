using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class CodafAnexoMap : BaseMapAuditavel<CodafAnexo>
    {
        public CodafAnexoMap()
        {
            ToTable("codaf_anexo");
            Map(c => c.CodafListaPresencaId).ToColumn("codaf_lista_presenca_id");
            Map(c => c.ArquivoCodigo).ToColumn("arquivo_codigo");
            Map(c => c.NomeArquivo).ToColumn("nome_arquivo");
            Map(c => c.Extensao).ToColumn("extensao");
            Map(c => c.TipoAnexoId).ToColumn("tipo_anexo_id");
            Map(c => c.CodafListaPresenca).Ignore();
        }
    }
}
