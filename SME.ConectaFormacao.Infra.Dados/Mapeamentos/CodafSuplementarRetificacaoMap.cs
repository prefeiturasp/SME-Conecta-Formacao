using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class CodafSuplementarRetificacaoMap : BaseMapAuditavel<CodafSuplementarRetificacao>
    {
        public CodafSuplementarRetificacaoMap()
        {
            ToTable("codaf_suplementar_retificacao");
            Map(c => c.CodafSuplementarId).ToColumn("codaf_suplementar_id");
            Map(c => c.PaginaRetificacaoDom).ToColumn("pagina_retificacao_dom");
            Map(c => c.DataRetificacao).ToColumn("data_retificacao");
            Map(c => c.CodafSuplementar).Ignore();
        }
    }
}
