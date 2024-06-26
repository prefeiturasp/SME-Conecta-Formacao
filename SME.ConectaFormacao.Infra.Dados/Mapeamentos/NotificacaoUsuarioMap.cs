﻿using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class NotificacaoUsuarioMap : BaseMapAuditavel<NotificacaoUsuario>
    {
        public NotificacaoUsuarioMap()
        {
            ToTable("notificacao_usuario");
            Map(c => c.NotificacaoId).ToColumn("notificacao_id");
            Map(c => c.Login).ToColumn("login");
            Map(c => c.Nome).ToColumn("nome");
            Map(c => c.Email).ToColumn("email");
            Map(c => c.Situacao).ToColumn("situacao");

            Map(c => c.Notificacao).Ignore();
        }
    }
}
