using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class UsuarioAcessibilidadeMap : BaseMapAuditavel<UsuarioAcessibilidade>
    {
        public UsuarioAcessibilidadeMap()
        {
            ToTable("usuario_acessibilidade");
            Map(c => c.UsuarioId).ToColumn("usuario_id");
            Map(c => c.PossuiDeficiencia).ToColumn("possui_deficiencia");
            Map(c => c.DescricaoDeficiencia).ToColumn("descricao_deficiencia");
            Map(c => c.NecessitaAdaptacao).ToColumn("necessita_adaptacao");
            Map(c => c.DescricaoAdaptacao).ToColumn("descricao_adaptacao");
        }
    }
}
