using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace SME.ConectaFormacao.TesteIntegracao.Api.Base
{
    public class TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory loggerFactory,
        UrlEncoder urlEncoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, loggerFactory, urlEncoder)
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var login = Request.Headers["x-test-Login"].ToString();

            if (string.IsNullOrEmpty(login))
                return Task.FromResult(AuthenticateResult.Fail("Login de teste não fornecido. Use o header 'x-test-Login' para fornecer um login."));

            var nome = Request.Headers["x-test-Nome"].ToString();
            var sistema = Request.Headers["x-test-Sistema"].ToString();
            var perfil = Request.Headers["x-test-Perfil"].ToString();
            var perfis = Request.Headers["x-test-Perfis"].ToString();
            var dres = Request.Headers["x-test-Dres"].ToString();
            var roles = Request.Headers["x-test-Roles"].ToString();

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, login),
                new("login", login),
                new("nome", string.IsNullOrWhiteSpace(nome) ? "Usuário de teste" : nome),
                new("sistema", string.IsNullOrWhiteSpace(sistema) ? "1007" : sistema),
                new("perfil", perfil)
            };

            if (!string.IsNullOrEmpty(perfis))
                foreach (var p in perfis.Split(',')) claims.Add(new Claim("perfis", p.Trim()));

            if (!string.IsNullOrEmpty(dres))
                foreach (var d in dres.Split(',')) claims.Add(new Claim("dres", d.Trim()));

            if (!string.IsNullOrEmpty(roles))
            {
                foreach (var r in roles.Split(','))
                {
                    claims.Add(new Claim("roles", r.Trim()));
                    claims.Add(new Claim(ClaimTypes.Role, r.Trim()));
                    claims.Add(new Claim("Permissoes", r.Trim()));
                }
            }

            var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var ticket = new AuthenticationTicket(principal, JwtBearerDefaults.AuthenticationScheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
