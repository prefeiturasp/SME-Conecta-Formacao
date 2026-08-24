using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Webapi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected IActionResult ProcessarResultado<T>(Resultado<T> resultado)
    {
        if (resultado.Sucesso)
        {
            return resultado.Dados is null ? NoContent() : Ok(resultado.Dados);
        }

        return ProcessarFalha(resultado);
    }

    protected IActionResult ProcessarResultado(Resultado resultado)
    {
        if (resultado.Sucesso)
        {
            return NoContent();
        }

        return ProcessarFalha(resultado);
    }

    protected IActionResult ProcessarCriado<T>(Resultado<T> resultado) => ProcessarCriado<T>(null, resultado);

    protected IActionResult ProcessarCriado<T>(string? uri, Resultado<T> resultado)
    {
        if (resultado.Sucesso)
        {
            return Created(uri, resultado.Dados);
        }

        return ProcessarFalha(resultado);
    }

    private IActionResult ProcessarFalha<T>(Resultado<T> resultado)
    {
        return resultado.TipoFalha switch
        {
            // *Alterado para o 422 pois está sendo omitido o corpo da resposta com o 404, e o front não consegue ler a mensagem de erro (by Diego Moreno - 2026-08-21)
            TipoFalha.NaoEncontrado => UnprocessableEntity(FormatarErro(resultado)),       // 404
            TipoFalha.Validacao => BadRequest(FormatarErro(resultado)),         // 400
            TipoFalha.RegraDeNegocio => UnprocessableEntity(FormatarErro(resultado)), // 422
            TipoFalha.NaoAutorizado => Forbid(resultado.MensagensErro.FirstOrDefault() ?? "Acesso negado"), // 403
            _ => StatusCode(500, FormatarErro(resultado))                       // 500
        };
    }

    private static object FormatarErro<T>(Resultado<T> resultado)
    {
        return new
        {
            sucesso = false,
            erros = resultado.MensagensErro
        };
    }
}