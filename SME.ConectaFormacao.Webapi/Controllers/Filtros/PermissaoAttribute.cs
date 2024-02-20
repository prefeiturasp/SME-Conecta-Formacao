using Microsoft.AspNetCore.Authorization;
using SME.ConectaFormacao.Dominio.Enumerados;
using System.Data;

namespace SME.ConectaFormacao.Webapi.Controllers.Filtros
{
    public class PermissaoAttribute : AuthorizeAttribute
    {
        public PermissaoAttribute(params Permissao[] permissoes)
        {
            var permissoesIds = permissoes.Select(x => (int)x);
            Roles = string.Join(",", permissoesIds);
        }
    }
}
