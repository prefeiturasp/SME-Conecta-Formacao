using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Interfaces.Relatorios;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos.Relatorios;
using SME.ConectaFormacao.Webapi.Controllers.Filtros;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    public class RelatorioController(ICasoDeUsoSolicitarGeracaoRelatorioInscritosPorFormacao casoDeUsoRelatorioInscritosPorFormacao) : BaseController
    {
        [HttpPost("inscritos-por-formacao")]
        [ProducesResponseType(typeof(Resultado), 202)]
        [Permissao(Permissao.Proposta_I, Permissao.Proposta_A, Permissao.Proposta_E, Policy = "Bearer")]
        public async Task<IActionResult> GerarRelatorioInscritosPorFormacao([FromBody] FiltroRelatorioInscritosPorFormacaoDto filtro)
        {
            var resultado = await casoDeUsoRelatorioInscritosPorFormacao.ExecutarAsync(filtro);
            
            if (resultado.Sucesso)
                return Accepted();

            return ProcessarResultado(resultado);
        }
    }
}
