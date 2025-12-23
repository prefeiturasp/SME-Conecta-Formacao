using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class CodafInscricaoMap : BaseMapAuditavel<CodafInscricaoListaPresenca>
    {
        public CodafInscricaoMap()
        {
            ToTable("codaf_inscricao");
            Map(c => c.CodafListaPresencaId).ToColumn("codaf_lista_presenca_id");
            Map(c => c.InscricaoId).ToColumn("inscricao_id");
            Map(c => c.PercentualFrequencia).ToColumn("percentual_frequencia");
            Map(c => c.AtividadeObrigatorio).ToColumn("atividade_obrigatorio");
            Map(c => c.ConceitoFinal).ToColumn("conceito_final");
            Map(c => c.Aprovado).ToColumn("aprovado");
            Map(c => c.Inscricao).Ignore();
            Map(c => c.CodafListaPresenca).Ignore();
        }
    }
}
