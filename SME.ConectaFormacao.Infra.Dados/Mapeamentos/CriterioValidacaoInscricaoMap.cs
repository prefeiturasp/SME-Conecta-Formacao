using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class CriterioValidacaoInscricaoMap : BaseMapAuditavel<CriterioValidacaoInscricao>
    {
        public CriterioValidacaoInscricaoMap()
        {
            ToTable("criterio_validacao_inscricao");
            Map(c => c.Nome).ToColumn("nome");
            Map(c => c.Unico).ToColumn("unico");
            Map(c => c.Outros).ToColumn("outros");
            Map(c => c.Ordem).ToColumn("ordem");
            Map(c => c.PermiteSorteio).ToColumn("permite_sorteio");
        }
    }
}
