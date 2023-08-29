using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    public class PropostaController : BaseController
    {
        [HttpGet("roteiro")]
        [ProducesResponseType(typeof(RoteiroPropostaFormativaDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterRoteiroPropostaFormativa([FromServices] ICasoDeUsoObterRoteiroPropostaFormativa casoDeUsoObterRoteiroPropostaFormativa)
        {
            return Ok(await casoDeUsoObterRoteiroPropostaFormativa.Executar());
        }

        [HttpGet("criterio-validacao-inscricao")]
        [ProducesResponseType(typeof(IEnumerable<CriterioValidacaoInscricaoDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterCriterioValidacaoInscricao([FromServices] ICasoDeUsoObterCriterioValidacaoInscricao casoDeUsoObterCriterioValidacaoInscricao)
        {
            return Ok(await casoDeUsoObterCriterioValidacaoInscricao.Executar());
        }

        [HttpGet("tipo-formacao")]
        [ProducesResponseType(typeof(IEnumerable<RetornoListagemDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterTipoFormacao([FromServices] ICasoDeUsoObterTipoFormacao casoDeUsoObterTipoFormacao)
        {
            return Ok(await casoDeUsoObterTipoFormacao.Executar());
        }
        
        [HttpGet("modalidades")]
        [ProducesResponseType(typeof(IEnumerable<RetornoListagemDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterModalidades([FromServices] ICasoDeUsoObterModalidades casoDeUsoObterModalidades)
        {
            return Ok(await casoDeUsoObterModalidades.Executar());
        }
    }
}
