using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class CodafRetificacaoMap : BaseMapAuditavel<CodafRetificacao>
    {
        public CodafRetificacaoMap()
        {
            ToTable("codaf_retificacao");
            Map(c => c.CodafListaPresencaId).ToColumn("codaf_lista_presenca_id");
            Map(c => c.PaginaRetificacaoDom).ToColumn("pagina_retificacao_dom");
            Map(c => c.DataRetificacao).ToColumn("data_retificacao");
            Map(c => c.CodafListaPresenca).Ignore();
        }
    }
}
