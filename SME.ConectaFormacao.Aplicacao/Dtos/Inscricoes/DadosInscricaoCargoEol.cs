namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes
{
    public class DadosInscricaoCargoEol
    {
        public string? Codigo { get; set; }
        public required string Descricao { get; set; }
        public required string DreCodigo { get; set; }
        public required string UeCodigo { get; set; }
        public int TipoVinculo { get; set; }
        public DateTime? DataInicio { get; set; }
        public List<DadosInscricaoCargoEol> Funcoes { get; set; } = [];
    }
}
