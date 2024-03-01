using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaEncontroMap : BaseMapAuditavel<PropostaEncontro>
    {
        public PropostaEncontroMap()
        {
            ToTable("proposta_encontro");

            Map(c => c.PropostaId).ToColumn("proposta_id");
            Map(c => c.HoraInicio).ToColumn("hora_inicio");
            Map(c => c.HoraFim).ToColumn("hora_fim");
            Map(c => c.Tipo).ToColumn("tipo");
            Map(c => c.Local).ToColumn("local");

            Map(c => c.Turmas).Ignore();
            Map(c => c.Datas).Ignore();
        }
    }
}
