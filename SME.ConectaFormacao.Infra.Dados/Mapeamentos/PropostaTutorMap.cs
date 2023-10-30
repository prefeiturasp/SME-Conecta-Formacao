using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaTutorMap : BaseMapAuditavel<PropostaTutor>
    {
        public PropostaTutorMap()
        {
            ToTable("proposta_tutor");
            Map(t => t.PropostaId).ToColumn("proposta_id");
            Map(t => t.ProfissionalRedeMunicipal).ToColumn("profissional_rede_municipal");
            Map(t => t.RegistroFuncional).ToColumn("registro_funcional");
            Map(t => t.NomeTutor).ToColumn("nome_tutor");
        }   
    }
}