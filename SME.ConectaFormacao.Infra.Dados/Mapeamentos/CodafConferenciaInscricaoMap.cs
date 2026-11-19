using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    [ExcludeFromCodeCoverage]
    public class CodafConferenciaInscricaoMap : BaseMapAuditavel<CodafConferenciaInscricao>
    {
        public CodafConferenciaInscricaoMap()
        {
            ToTable("codaf_conferencia_inscricao");
            Map(c => c.CodafConferenciaId).ToColumn("codaf_conferencia_id");
            Map(c => c.InscricaoId).ToColumn("inscricao_id");
            Map(c => c.Participou).ToColumn("participou");
            Map(c => c.CodafConferencia).Ignore();
            Map(c => c.Inscricao).Ignore();
        }
    }
}
