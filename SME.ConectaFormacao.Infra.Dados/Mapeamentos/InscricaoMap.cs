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
            Map(c => c.CodigoCargoEol).ToColumn("codigo_cargo_eol");
            Map(c => c.CargoEol).ToColumn("cargo_eol");
            Map(c => c.CodigoTipoFuncaoEol).ToColumn("codigo_tipo_funcao_eol");
            Map(c => c.TipoFuncaoEol).ToColumn("tipo_funcao_eol");
            Map(c => c.ArquivoId).ToColumn("arquivo_id");
            Map(c => c.Situacao).ToColumn("situacao");
        }
    }
}
