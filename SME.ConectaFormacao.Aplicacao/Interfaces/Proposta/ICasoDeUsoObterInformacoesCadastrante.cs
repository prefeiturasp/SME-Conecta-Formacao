﻿using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterInformacoesCadastrante
    {
        Task<PropostaInformacoesCadastranteDTO> Executar(long? propostaId);
    }
}
