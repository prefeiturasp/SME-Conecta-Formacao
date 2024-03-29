﻿using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioPalavraChave : IRepositorioBaseAuditavel<PalavraChave>
    {
        Task<IEnumerable<PalavraChave>> ObterLista();
    }
}
