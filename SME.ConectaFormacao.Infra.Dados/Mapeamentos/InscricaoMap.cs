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
            Map(c => c.FuncaoId).ToColumn("funcao_id");
            Map(c => c.ArquivoId).ToColumn("arquivo_id");
            Map(c => c.Situacao).ToColumn("situacao");
        }
    }
}
