﻿namespace SME.ConectaFormacao.Infra.Servicos.Acessos
{
    public class AcessosPerfisUsuarioRetorno
    {
        public string UsuarioNome { get; set; }
        public string UsuarioLogin { get; set; }
        public string Cpf { get; set; }
        public DateTime DataHoraExpiracao { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
        public bool Autenticado { get; set; }
        public IList<AcessosPerfilUsuario> PerfilUsuario { get; set; }
    }
}