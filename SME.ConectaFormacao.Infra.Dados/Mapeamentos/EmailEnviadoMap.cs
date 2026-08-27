using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class EmailEnviadoMap : BaseMapAuditavel<EmailEnviado>
    {
        public EmailEnviadoMap()
        {
            ToTable("email_enviado");

            Map(c => c.ChaveIdempotencia).ToColumn("chave_idempotencia");
            Map(c => c.EmailDestinatario).ToColumn("email_destinatario");
            Map(c => c.NomeDestinatario).ToColumn("nome_destinatario");
            Map(c => c.Titulo).ToColumn("titulo");
            Map(c => c.ConteudoHash).ToColumn("conteudo_hash");
            Map(c => c.EnviadoEm).ToColumn("enviado_em");
            Map(c => c.NotificacaoUsuarioId).ToColumn("notificacao_usuario_id");
            Map(c => c.TentativasEnvio).ToColumn("tentativas_envio");
            Map(c => c.MensagemErro).ToColumn("mensagem_erro");

            Map(c => c.NotificacaoUsuario).Ignore();
        }
    }
}
