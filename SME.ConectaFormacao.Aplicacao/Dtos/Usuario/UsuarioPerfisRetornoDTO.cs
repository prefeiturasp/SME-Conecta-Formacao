﻿namespace SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
public class UsuarioPerfisRetornoDTO
{
    public string UsuarioNome { get; set; }
    public string UsuarioLogin { get; set; }
    public DateTime DataHoraExpiracao { get; set; }
    public string Token { get; set; }
    public string Email { get; set; }
    public string Cpf { get; set; }
    public bool Autenticado { get; set; }
    public IList<PerfilUsuarioDTO> PerfilUsuario { get; set; }
}
