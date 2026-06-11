namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes
{
    public class DadosInscricaoDto
    {
        public required string UsuarioNome { get; set; }
        public required string UsuarioRf { get; set; }
        public string? UsuarioCpf { get; set; }
        public string? UsuarioEmail { get; set; }
        public string? UsuarioTelefone { get; set; }
        public IEnumerable<DadosInscricaoCargoEol> UsuarioCargos { get; set; } = [];
    }
}
