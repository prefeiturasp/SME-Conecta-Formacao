﻿using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoInserirProposta
    {
        Task<long> Executar(PropostaDTO propostaDTO);
    }
}
