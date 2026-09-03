using Microsoft.Extensions.Primitives;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Enumerados;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace SME.ConectaFormacao.Webapi.Contexto;

[ExcludeFromCodeCoverage]
public class ContextoHttp : ContextoBase
{
    readonly IHttpContextAccessor httpContextAccessor;

    public ContextoHttp(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;

        CapturarVariaveis();
    }

    private void CapturarVariaveis()
    {
        var userContext = httpContextAccessor.HttpContext?.User;
        Variaveis.Add("RF", userContext?.FindFirst("RF")?.Value ?? "0");
        Variaveis.Add("Claims", GetInternalClaim());
        Variaveis.Add("login", userContext?.Claims?.FirstOrDefault(a => a.Type == "login")?.Value ?? string.Empty);
        Variaveis.Add("UsuarioLogado", userContext?.Identity?.Name ?? "Sistema");
        Variaveis.Add("NomeUsuario", userContext?.FindFirst("Nome")?.Value ?? userContext?.FindFirst("nome")?.Value ?? "Sistema");
        Variaveis.Add("PerfilUsuario", ObterPerfilAtual());
        Variaveis.Add("EmailUsuario", userContext?.Claims?.FirstOrDefault(a => a.Type == "email")?.Value ?? string.Empty);
        Variaveis.Add("Dres", userContext?.Claims?.Where(a => a.Type == "dres").Select(s => s.Value).ToArray() ?? []);
        Variaveis.Add("Permissoes", ObterPermissoes());

        var authorizationHeader = httpContextAccessor.HttpContext?.Request?.Headers["authorization"];

        if (!authorizationHeader.HasValue || authorizationHeader.Value == StringValues.Empty)
        {
            Variaveis.Add("TemAuthorizationHeader", false);
            Variaveis.Add("TokenAtual", string.Empty);
        }
        else
        {
            Variaveis.Add("TemAuthorizationHeader", true);
            Variaveis.Add("TokenAtual", authorizationHeader.Value.Single()!.Split(' ')[^1]);
        }

        var numeroPagina = httpContextAccessor.HttpContext?.Request?.Headers["numeroPagina"].ToString();
        if (!string.IsNullOrEmpty(numeroPagina))
            Variaveis.Add("NumeroPagina", numeroPagina);

        var numeroRegistros = httpContextAccessor.HttpContext?.Request?.Headers["numeroRegistros"].ToString();
        if (!string.IsNullOrEmpty(numeroRegistros))
            Variaveis.Add("NumeroRegistros", numeroRegistros);
    }

    private IEnumerable<InternalClaim> GetInternalClaim()
    {
        return (httpContextAccessor.HttpContext?.User?.Claims ?? Enumerable.Empty<Claim>()).Select(x => new InternalClaim() { Type = x.Type, Value = x.Value }).ToList();
    }

    private string ObterPerfilAtual()
    {
        return (httpContextAccessor.HttpContext?.User?.Claims ?? Enumerable.Empty<Claim>()).FirstOrDefault(x => x.Type.ToLower() == "perfil")?.Value;
    }

    public override IContextoAplicacao AtribuirContexto(IContextoAplicacao contexto)
    {
        throw new Exception("Este tipo de conexto não permite atribuição");
    }

    public override void AdicionarVariaveis(IDictionary<string, object> variaveis)
    {
        this.Variaveis = variaveis;
    }
    private Permissao[] ObterPermissoes()
    {
        return [.. (httpContextAccessor.HttpContext?.User?.Claims ?? [])
                .Where(x => x.Type == ClaimTypes.Role)
                .Select(x => Enum.TryParse<Permissao>(x.Value, out var permissao) && Enum.IsDefined(permissao)
                        ? permissao
                        : (Permissao?)null)
                .OfType<Permissao>()];
    }
}

