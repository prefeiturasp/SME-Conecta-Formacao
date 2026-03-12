using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Interfaces.Ues;
using SME.ConectaFormacao.Infra.Dados.Dtos.Ues;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    public class UeController (ICasoDeUsoObterAutocompletarNomeUe casoDeUsoAutocompletarNome) : BaseController
    {
        [HttpGet("autocompletar-nome")]
        public async Task<IActionResult> AutocompletarNomeAsync([FromQuery] FiltroAutocompletarNomeUeDto filtro)
        {
            return ProcessarResultado(await casoDeUsoAutocompletarNome.ExecutarAsync(filtro));
        }
    }
}
