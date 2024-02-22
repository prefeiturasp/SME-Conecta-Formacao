using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaRegenteMap : BaseMapAuditavel<PropostaRegente>
    {
        public PropostaRegenteMap()
        {
            ToTable("proposta_regente");
            Map(t => t.PropostaId).ToColumn("proposta_id");
            Map(t => t.ProfissionalRedeMunicipal).ToColumn("profissional_rede_municipal");
            Map(t => t.RegistroFuncional).ToColumn("registro_funcional");
            Map(t => t.Cpf).ToColumn("cpf");
            Map(t => t.NomeRegente).ToColumn("nome_regente");
            Map(t => t.MiniBiografia).ToColumn("mini_biografia");
        }
    }
}