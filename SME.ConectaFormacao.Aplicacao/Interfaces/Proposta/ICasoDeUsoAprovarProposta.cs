﻿using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoAprovarProposta
    {
        Task<bool> Executar(long propostaId, PropostaJustificativaDTO propostaJustificativaDTO);
    }
}
