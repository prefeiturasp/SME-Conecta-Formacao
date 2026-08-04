namespace SME.ConectaFormacao.Infra.Servicos.Eol
{
    public class UsuarioEolDto
    {
        public string Login { get; set; } = null!;
        public string Nome { get; set; } = null!;
        public string? NomeSocial { get; set; }
    }
}
