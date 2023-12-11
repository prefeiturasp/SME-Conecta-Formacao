using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SME.ConectaFormacao.Aplicacao.DTOS;

namespace SME.ConectaFormacao.Webapi.Filtros
{
    public class ValidaDtoAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new FalhaValidacaoResult(context.ModelState);
            }
        }

        public class FalhaValidacaoResult : ObjectResult
        {
            public FalhaValidacaoResult(ModelStateDictionary modelState)
                : base(RetornaBaseModel(modelState))
            {
                StatusCode = StatusCodes.Status422UnprocessableEntity;
            }

            public static RetornoBaseDTO RetornaBaseModel(ModelStateDictionary modelState)
            {
                var dto = new RetornoBaseDTO();
                dto.Mensagens = modelState.Keys
                       .SelectMany(key => modelState[key].Errors.Select(x => new string(x.ErrorMessage)))
                       .ToList();
                return dto;
            }
        }
    }
}