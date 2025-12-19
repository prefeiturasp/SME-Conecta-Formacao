using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    public class CodafListaPresencaController(
        ICasoDeUsoCriarCodafListaPresenca casoDeUsoCriarCodafListaPresenca,
        ICasoDeUsoAtualizarCodafListaPresenca casoDeUsoAtualizar) : BaseController
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
    }
}