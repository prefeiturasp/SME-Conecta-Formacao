namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao
{
    public class DadosInscricaoDTO
    {
        public string UsuarioNome { get; set; }
        public string UsuarioRf { get; set; }
        public string UsuarioCpf { get; set; }
        public string UsuarioEmail { get; set; }
        public IEnumerable<DadosInscricaoCargoEol> UsuarioCargos { get; set; }
    }

    public class DadosInscricaoCargoEol
    {
        public string? Codigo { get; set; }
        public string Descricao { get; set; }
        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public int TipoVinculo { get; set; }
        public DateTime? DataInicio { get; set; }
        public List<DadosInscricaoCargoEol> Funcoes { get; set; } = new();
    }
}
