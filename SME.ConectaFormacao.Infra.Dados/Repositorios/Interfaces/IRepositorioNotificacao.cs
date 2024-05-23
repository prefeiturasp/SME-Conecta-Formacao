﻿using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioNotificacao : IRepositorioBaseAuditavel<Notificacao>
    {
        Task<long> ObterTotalNaoLidoPorUsuario(string login);
    }
}
