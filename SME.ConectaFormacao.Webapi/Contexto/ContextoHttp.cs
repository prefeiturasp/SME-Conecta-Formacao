﻿using System.Security.Claims;
using Microsoft.Extensions.Primitives;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Webapi.Contexto;

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
            Variaveis.Add("RF", httpContextAccessor.HttpContext?.User?.FindFirst("RF")?.Value ?? "0");
            Variaveis.Add("Claims", GetInternalClaim());
            Variaveis.Add("login", httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(a => a.Type == "login")?.Value ?? string.Empty);
            Variaveis.Add("UsuarioLogado", httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Sistema");
            Variaveis.Add("NomeUsuario", httpContextAccessor.HttpContext?.User?.FindFirst("Nome")?.Value ?? "Sistema");
            Variaveis.Add("PerfilUsuario", ObterPerfilAtual());
            
            var authorizationHeader = httpContextAccessor.HttpContext?.Request?.Headers["authorization"];

            if (!authorizationHeader.HasValue || authorizationHeader.Value == StringValues.Empty)
            {
                Variaveis.Add("TemAuthorizationHeader", false);
                Variaveis.Add("TokenAtual", string.Empty);
            }
            else
            {
                Variaveis.Add("TemAuthorizationHeader", true);
                Variaveis.Add("TokenAtual", authorizationHeader.Value.Single().Split(' ').Last());
            }
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
    }
