using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    [ExcludeFromCodeCoverage]
    public class CodafConferenciaAnexoMap : BaseMapAuditavel<CodafConferenciaAnexo>
    {
        public CodafConferenciaAnexoMap()
        {
            ToTable("codaf_conferencia_anexo");
            Map(c => c.CodafConferenciaId).ToColumn("codaf_conferencia_id");
            Map(c => c.ArquivoCodigo).ToColumn("arquivo_codigo");
            Map(c => c.NomeArquivo).ToColumn("nome_arquivo");
            Map(c => c.Extensao).ToColumn("extensao");
            Map(c => c.TipoAnexoId).ToColumn("tipo_anexo_id");
            Map(c => c.CodafConferencia).Ignore();
        }
    }
}
