using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaEncontroTurmaMap : BaseMapAuditavel<PropostaEncontroTurma>
    {
        public PropostaEncontroTurmaMap()
        {
            ToTable("proposta_encontro_turma");

            Map(t => t.PropostaEncontroId).ToColumn("proposta_encontro_id");
            Map(t => t.Turma).ToColumn("turma");
        }
    }
}
