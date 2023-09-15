using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class ArquivoMap : BaseMapAuditavel<Arquivo>
    {
        public ArquivoMap()
        {
            ToTable("arquivo");
            Map(c => c.Nome).ToColumn("nome");
            Map(c => c.Codigo).ToColumn("codigo");
            Map(c => c.TipoConteudo).ToColumn("tipo_conteudo");
            Map(c => c.Tipo).ToColumn("tipo");

            Map(c => c.NomeArquivoFisico).Ignore();
            Map(c => c.EhTemp).Ignore();
        }
    }
}
