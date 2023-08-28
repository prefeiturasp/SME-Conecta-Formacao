﻿using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    public class PropostaController : BaseController
    {
        [HttpGet("roteiro")]
        [ProducesResponseType(typeof(RoteiroPropostaFormativaDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        //[Authorize("Bearer")]
        public async Task<IActionResult> ObterRoteiroPropostaFormativa([FromServices] ICasoDeUsoObterRoteiroPropostaFormativa casoDeUsoObterRoteiroPropostaFormativa)
        {
            return Ok(await casoDeUsoObterRoteiroPropostaFormativa.Executar());
        }
    }
}
