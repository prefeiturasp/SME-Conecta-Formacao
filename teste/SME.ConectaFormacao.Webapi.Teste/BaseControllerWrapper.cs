using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Webapi.Controllers;

namespace SME.ConectaFormacao.Webapi.Teste
{
    public class BaseControllerWrapper : BaseController
    {
        public new IActionResult ProcessarResultado<T>(Resultado<T> resultado)
        => base.ProcessarResultado(resultado);

        public new IActionResult ProcessarResultado(Resultado resultado)
            => base.ProcessarResultado(resultado);

        public new IActionResult ProcessarCriado<T>(string? uri, Resultado<T> resultado)
            => base.ProcessarCriado(uri, resultado);
    }
}