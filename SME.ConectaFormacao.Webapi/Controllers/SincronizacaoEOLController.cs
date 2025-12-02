using MediatR;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SincronizacaoEOLController : BaseController
    {
        public SincronizacaoEOLController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// Executa a sincronização de Função Atividade do EOL para todas as DREs
        /// </summary>
        /// <returns>Mensagem de sucesso</returns>
        [HttpPost("funcao-atividade")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> SincronizarFuncaoAtividade()
        {
            var resultado = await mediator.Send(new PublicarNaFilaRabbitCommand(
                RotasRabbit.SincronizaFuncaoAtividade,
                string.Empty));

            if (resultado)
                return Ok("Sincronização de Função Atividade iniciada com sucesso!");

            return BadRequest("Erro ao publicar mensagem na fila");
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

            var resultado = await mediator.Send(new PublicarNaFilaRabbitCommand(
                RotasRabbit.SincronizaFuncaoAtividadeDre,
                codigoDre));

            if (resultado)
                return Ok($"Sincronização de Função Atividade para DRE {codigoDre} iniciada com sucesso!");

            return BadRequest($"Erro ao publicar mensagem na fila para DRE {codigoDre}");
        }

        /// <summary>
        /// Executa a sincronização de Cargos do EOL para todas as DREs
        /// </summary>
        /// <returns>Mensagem de sucesso</returns>
        [HttpPost("cargos")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> SincronizarCargos()
        {
            var resultado = await mediator.Send(new PublicarNaFilaRabbitCommand(
                RotasRabbit.SincronizaCargosEol,
                string.Empty));

            if (resultado)
                return Ok("Sincronização de Cargos EOL iniciada com sucesso!");

            return BadRequest("Erro ao publicar mensagem na fila");
        }
    }
}
