using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos.Arquivo;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.Arquivo;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Webapi.Controllers.Filtros;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    public class ArquivoController : BaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(ArquivoArmazenadoDTO), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Permissao(Permissao.Proposta_C, Permissao.Proposta_I, Permissao.Proposta_A, Permissao.Proposta_E, Policy = "Bearer")]
        public async Task<IActionResult> Carregar(
            [FromServices] ICasoDeUsoArquivoCarregarTemporario casoDeUsoArquivoCarregarTemporario,
            IFormFile file)
        {
            return Ok(await casoDeUsoArquivoCarregarTemporario.Executar(file));
        }

        [HttpDelete]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Permissao(Permissao.Proposta_C, Permissao.Proposta_I, Permissao.Proposta_A, Permissao.Proposta_E, Policy = "Bearer")]
        public async Task<IActionResult> Excluir(
            [FromServices] ICasoDeUsoArquivoExcluir casoDeUsoArquivoExcluir,
            [FromBody] Guid[] codigos)
        {
            return Ok(await casoDeUsoArquivoExcluir.Executar(codigos));
        }

        [HttpGet("{codigoArquivo}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Permissao(Permissao.Proposta_C, Permissao.Proposta_I, Permissao.Proposta_A, Permissao.Proposta_E, Policy = "Bearer")]
        public async Task<IActionResult> Baixar(
            [FromServices] ICasoDeUsoArquivoBaixar casoDeUsoArquivoBaixar,
            Guid codigoArquivo)
        {
            var arquivoBaixadoDTO = await casoDeUsoArquivoBaixar.Executar(codigoArquivo);
            if (arquivoBaixadoDTO == null) return NoContent();

            return File(arquivoBaixadoDTO.Arquivo, arquivoBaixadoDTO.TipoConteudo, arquivoBaixadoDTO.Nome);
        }
    }
}
