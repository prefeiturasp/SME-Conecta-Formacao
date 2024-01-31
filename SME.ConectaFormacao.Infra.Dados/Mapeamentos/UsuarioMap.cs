using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class UsuarioMap : BaseMapAuditavel<Usuario>
    {
        public UsuarioMap()
        {
            ToTable("usuario");
            Map(c => c.Login).ToColumn("login");
            Map(c => c.Nome).ToColumn("nome");
            Map(c => c.Email).ToColumn("email");
            Map(c => c.UltimoLogin).ToColumn("ultimo_login");
            Map(c => c.TokenRecuperacaoSenha).ToColumn("token_recuperacao_senha");
            Map(c => c.ExpiracaoRecuperacaoSenha).ToColumn("expiracao_recuperacao_senha");
            Map(c => c.Cpf).ToColumn("cpf");
            Map(c => c.UeCodigo).ToColumn("ue_codigo");
            Map(c => c.Tipo).ToColumn("tipo");
            Map(c => c.PossuiContratoExterno).ToColumn("possui_contrato_externo");
            Map(c => c.Situacao).ToColumn("situacao_cadastro");
        }
    }
}
