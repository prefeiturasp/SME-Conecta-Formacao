using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos.Grupo;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.Grupo;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Webapi.Controllers.Filtros;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    public class GrupoController : BaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GrupoDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Permissao(Permissao.AreaPromotora_C, Permissao.AreaPromotora_I, Permissao.AreaPromotora_A, Permissao.AreaPromotora_E, Policy = "Bearer")]
        public async Task<IActionResult> ObterGrupos([FromServices] ICasoDeUsoObterGrupoSistema casoDeUsoObterGrupoSistema)
        {
            return Ok(await casoDeUsoObterGrupoSistema.Executar());
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GrupoDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Permissao(Permissao.AreaPromotora_C, Permissao.AreaPromotora_I, Permissao.AreaPromotora_A, Permissao.AreaPromotora_E, Policy = "Bearer")]
        public async Task<IActionResult> ObterGrupoGestao([FromServices] ICasoDeUsoObterGrupoGestao casoDeUsoObterGrupoGestao)
        {
            return Ok(await casoDeUsoObterGrupoGestao.Executar());
        }
    }
}
