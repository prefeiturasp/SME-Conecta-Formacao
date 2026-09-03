using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Interfaces.SincronizacaoEOL;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SincronizacaoEOLController(
        ISincronizarFuncaoAtividadeEolUseCase sincronizarFuncaoAtividadeEolUseCase,
        ISincronizarFuncaoAtividadeEolPorDreUseCase sincronizarFuncaoAtividadeEolPorDreUseCase,
        IExecutarSincronizacaoCargosEolUseCase executarSincronizacaoCargosEolUseCase) : BaseController
    {

        /// <summary>
        /// Executa a sincronização de Função Atividade do EOL para todas as DREs
        /// </summary>
        /// <returns>Mensagem de sucesso</returns>
        [HttpPost("funcao-atividade")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> SincronizarFuncaoAtividade()
        {
            // Chamada direta ao Use Case
            var resultado = await sincronizarFuncaoAtividadeEolUseCase.Executar(new MensagemRabbit(new { }));

            if (resultado)
                return Ok("Sincronização de Função Atividade iniciada com sucesso!");

            return BadRequest("Erro ao executar sincronização");
        }

        /// <summary>
        /// Executa a sincronização de Função Atividade do EOL para uma DRE específica
        /// </summary>
        /// <param name="codigoDre">Código da DRE (ex: DRE-BT, DRE-CL, SME)</param>
        /// <returns>Mensagem de sucesso</returns>
        [HttpPost("funcao-atividade/{codigoDre}")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> SincronizarFuncaoAtividadePorDre(string codigoDre)
        {
            if (string.IsNullOrWhiteSpace(codigoDre))
                return BadRequest("Código da DRE é obrigatório");

            // Chamada direta ao Use Case
            var resultado = await sincronizarFuncaoAtividadeEolPorDreUseCase.Executar(new MensagemRabbit(codigoDre));

            if (resultado)
                return Ok($"Sincronização de Função Atividade para DRE {codigoDre} executada com sucesso!");

            return BadRequest($"Erro ao executar sincronização para DRE {codigoDre}");
        }

        /// <summary>
        /// Executa a sincronização de Cargos do EOL para todas as DREs
        /// </summary>
        /// <returns>Mensagem de sucesso</returns>
        [HttpPost("cargos")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> SincronizarCargos()
        {
            // Chamada direta ao Use Case
            var resultado = await executarSincronizacaoCargosEolUseCase.Executar(new MensagemRabbit(new { }));

            if (resultado)
                return Ok("Sincronização de Cargos EOL executada com sucesso!");

            return BadRequest("Erro ao executar sincronização");
        }
    }
}
