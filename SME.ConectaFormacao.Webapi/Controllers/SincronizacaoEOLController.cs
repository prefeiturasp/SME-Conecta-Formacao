using MediatR;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Interfaces.SincronizacaoEOL;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SincronizacaoEOLController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly ISincronizarFuncaoAtividadeEolUseCase _sincronizarFuncaoAtividadeEolUseCase;
        private readonly ISincronizarFuncaoAtividadeEolPorDreUseCase _sincronizarFuncaoAtividadeEolPorDreUseCase;
        private readonly IExecutarSincronizacaoCargosEolUseCase _executarSincronizacaoCargosEolUseCase;

        public SincronizacaoEOLController(IMediator mediator,
            ISincronizarFuncaoAtividadeEolUseCase sincronizarFuncaoAtividadeEolUseCase,
            ISincronizarFuncaoAtividadeEolPorDreUseCase sincronizarFuncaoAtividadeEolPorDreUseCase,
            IExecutarSincronizacaoCargosEolUseCase executarSincronizacaoCargosEolUseCase)
        {
            _mediator = mediator;
            _sincronizarFuncaoAtividadeEolUseCase = sincronizarFuncaoAtividadeEolUseCase;
            _sincronizarFuncaoAtividadeEolPorDreUseCase = sincronizarFuncaoAtividadeEolPorDreUseCase;
            _executarSincronizacaoCargosEolUseCase = executarSincronizacaoCargosEolUseCase;
        }

        /// <summary>
        /// Executa a sincronização de Função Atividade do EOL para todas as DREs
        /// </summary>
        /// <returns>Mensagem de sucesso</returns>
        [HttpPost("funcao-atividade")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> SincronizarFuncaoAtividade()
        {
            // Chamada direta ao Use Case
            var resultado = await _sincronizarFuncaoAtividadeEolUseCase.Executar(new MensagemRabbit(new { }));

            if (resultado)
                return Ok("Sincronização de Função Atividade iniciada com sucesso!");

            return BadRequest("Erro ao executar sincronização");

            // Código original (via fila RabbitMQ) - comentado
            //var resultado = await _mediator.Send(new PublicarNaFilaRabbitCommand(
            //    RotasRabbit.SincronizaFuncaoAtividade,
            //    new { }));
            //
            //if (resultado)
            //    return Ok("Sincronização de Função Atividade iniciada com sucesso!");
            //
            //return BadRequest("Erro ao publicar mensagem na fila");
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
            var resultado = await _sincronizarFuncaoAtividadeEolPorDreUseCase.Executar(new MensagemRabbit(codigoDre));

            if (resultado)
                return Ok($"Sincronização de Função Atividade para DRE {codigoDre} executada com sucesso!");

            return BadRequest($"Erro ao executar sincronização para DRE {codigoDre}");

            // Código original (via fila RabbitMQ) - comentado
            //var resultado = await _mediator.Send(new PublicarNaFilaRabbitCommand(
            //    RotasRabbit.SincronizaFuncaoAtividadeDre,
            //    codigoDre));
            //
            //if (resultado)
            //    return Ok($"Sincronização de Função Atividade para DRE {codigoDre} iniciada com sucesso!");
            //
            //return BadRequest($"Erro ao publicar mensagem na fila para DRE {codigoDre}");
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
            var resultado = await _executarSincronizacaoCargosEolUseCase.Executar(new MensagemRabbit(new { }));

            if (resultado)
                return Ok("Sincronização de Cargos EOL executada com sucesso!");

            return BadRequest("Erro ao executar sincronização");

            // Código original (via fila RabbitMQ) - comentado
            //var resultado = await _mediator.Send(new PublicarNaFilaRabbitCommand(
            //    RotasRabbit.SincronizaCargosEol,
            //    new { }));
            //
            //if (resultado)
            //    return Ok("Sincronização de Cargos EOL iniciada com sucesso!");
            //
            //return BadRequest("Erro ao publicar mensagem na fila");
        }
    }
}
