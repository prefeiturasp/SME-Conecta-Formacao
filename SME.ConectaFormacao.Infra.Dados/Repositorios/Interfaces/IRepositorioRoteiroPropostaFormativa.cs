﻿using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioRoteiroPropostaFormativa : IRepositorioBaseAuditavel<RoteiroPropostaFormativa>
    {
        Task<RoteiroPropostaFormativa> ObterUltimoRoteiroAtivo();
    }
}
