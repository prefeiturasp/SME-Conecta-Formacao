using Microsoft.AspNetCore.Authorization;
using SME.ConectaFormacao.Dominio.Enumerados;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Webapi.Controllers.Filtros
{
    [ExcludeFromCodeCoverage]
    public class PermissaoAttribute : AuthorizeAttribute
    {
        public PermissaoAttribute(params Permissao[] permissoes)
        {
            var permissoesIds = permissoes.Select(x => (int)x);
            Roles = string.Join(",", permissoesIds);
        }
    }
}
