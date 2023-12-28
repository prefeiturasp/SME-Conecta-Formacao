using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class InscricaoMap : BaseMapAuditavel<Inscricao>
    {
        public InscricaoMap()
        {
            ToTable("inscricao");
            Map(c => c.PropostaTurmaId).ToColumn("proposta_turma_id");
            Map(c => c.UsuarioId).ToColumn("usuario_id");
            
            Map(c => c.CargoId).ToColumn("cargo_id");
            Map(c => c.CargoCodigo).ToColumn("cargo_codigo");
            Map(c => c.CargoDreCodigo).ToColumn("cargo_dre_codigo");
            Map(c => c.CargoUeCodigo).ToColumn("cargo_ue_codigo");
            
            Map(c => c.FuncaoId).ToColumn("funcao_id");
            Map(c => c.CargoCodigo).ToColumn("funcao_codigo");
            Map(c => c.CargoDreCodigo).ToColumn("funcao_dre_codigo");
            Map(c => c.CargoUeCodigo).ToColumn("funcao_ue_codigo");
            
            Map(c => c.ArquivoId).ToColumn("arquivo_id");
            Map(c => c.Situacao).ToColumn("situacao");
        }
    }
}
