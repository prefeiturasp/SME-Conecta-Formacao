﻿namespace SME.ConectaFormacao.Infra.Servicos.Eol
{
    public class UsuarioPerfilServicoEol
    {
        public string Login { get; set; }
        public string Nome { get; set; } = string.Empty;
        public Guid Perfil { get; set; }
    }
}