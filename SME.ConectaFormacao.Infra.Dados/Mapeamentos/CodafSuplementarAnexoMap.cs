using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class CodafSuplementarAnexoMap : BaseMapAuditavel<CodafSuplementarAnexo>
    {
        public CodafSuplementarAnexoMap()
        {
            ToTable("codaf_suplementar_anexo");
            Map(c => c.CodafSuplementarId).ToColumn("codaf_suplementar_id");
            Map(c => c.ArquivoCodigo).ToColumn("arquivo_codigo");
            Map(c => c.NomeArquivo).ToColumn("nome_arquivo");
            Map(c => c.Extensao).ToColumn("extensao");
            Map(c => c.TipoAnexoId).ToColumn("tipo_anexo_id");
            Map(c => c.CodafSuplementar).Ignore();
        }
    }
}
