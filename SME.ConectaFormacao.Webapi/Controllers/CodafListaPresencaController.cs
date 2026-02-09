using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    [Route("api/v1/CodafListaPresenca")]
    public class CodafListaPresencaController(
        ICasoDeUsoCriarCodafListaPresenca casoDeUsoCriarCodafListaPresenca,
        ICasoDeUsoAtualizarCodafListaPresenca casoDeUsoAtualizar,
        ICasoDeUsoListarCodafListaPresenca casoDeUsoListarCodafListaPresenca,
        ICasoDeUsoObterCodafListaPresencaPorId casoDeUsoObterCodafListaPresencaPorId,
        ICasoDeUsoRemoverCodafRetificacaoListaPresenca casoDeUsoRemoverCodafRetificacaoListaPresenca,
        ICasoDeUsoExcluirCodafListaPresenca casoDeUsoExcluirCodafListaPresenca,
        ICasoDeUsoGerarRelatorioCodaf casoDeUsoGerarRelatorioCodaf) : BaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(Resultado<CodafListaPresencaDto>), 201)]
        [ProducesResponseType(typeof(Resultado<CodafListaPresencaDto>), 400)]
        [ProducesResponseType(typeof(Resultado<CodafListaPresencaDto>), 404)]
        [ProducesResponseType(typeof(Resultado<CodafListaPresencaDto>), 422)]
        public async Task<IActionResult> Cadastrar([FromBody] CodafListaPresencaCadastroDto codafListaPresencaCadastroDto)
        {
            var resultado = await casoDeUsoCriarCodafListaPresenca.ExecutarAsync(codafListaPresencaCadastroDto);
            return ProcessarCriado(resultado);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(Resultado), 204)]
        [ProducesResponseType(typeof(Resultado), 400)]
        [ProducesResponseType(typeof(Resultado), 404)]
        [ProducesResponseType(typeof(Resultado), 422)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] CodafListaPresencaEdicaoDto codafListaPresencaEdicaoDto)
        {
            var resultado = await casoDeUsoAtualizar.ExecutarAsync(codafListaPresencaEdicaoDto, id);
            return ProcessarResultado(resultado);
        }

        [HttpGet]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<ListaPresencaCodafResumoDto>>), 200)]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<ListaPresencaCodafResumoDto>>), 400)]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<ListaPresencaCodafResumoDto>>), 500)]
        public async Task<IActionResult> ObterListaPaginada([FromQuery] FiltroListaPresencaCodafDto filtro)
        {
            var resultado = await casoDeUsoListarCodafListaPresenca.ExecutarAsync(filtro);
            return ProcessarResultado(resultado);
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(Resultado<CodafListaPresencaDto>), 200)]
        [ProducesResponseType(typeof(Resultado<CodafListaPresencaDto>), 404)]
        public async Task<IActionResult> ObterPorId(long id)
        {
            var resultado = await casoDeUsoObterCodafListaPresencaPorId.ExecutarAsync(id);
            return ProcessarResultado(resultado);
        }

        [HttpDelete("retificacoes/{retificacaoId:long}")]
        [ProducesResponseType(typeof(Resultado<bool>), 204)]
        [ProducesResponseType(typeof(Resultado<bool>), 404)]
        public async Task<IActionResult> RemoverRetificacao(long retificacaoId)
        {
            var resultado = await casoDeUsoRemoverCodafRetificacaoListaPresenca.ExecutarAsync(retificacaoId);
            return ProcessarResultado(resultado);
        }

        [HttpDelete("{codafListaPresencaId:long}")]
        public async Task<IActionResult> Excluir(long codafListaPresencaId)
        {
            var resultado = await casoDeUsoExcluirCodafListaPresenca.ExecutarAsync(codafListaPresencaId);
            return ProcessarResultado(resultado);
        }

        [HttpGet("{codafId:long}/imprimir")]
        [ProducesResponseType(typeof(Resultado<ArquivoDto>), 200)]
        [ProducesResponseType(typeof(Resultado<ArquivoDto>), 404)]
        public async Task<IActionResult> ImprimirRelatorioCodafAsync(long codafId)
        {
            var resultado = await casoDeUsoGerarRelatorioCodaf.ExecutarAsync(codafId);

            if (resultado.Sucesso)
                return File(resultado.Dados!.Stream, resultado.Dados.ContentType, resultado.Dados.Nome);

            return ProcessarResultado(resultado);
        }
    }
}