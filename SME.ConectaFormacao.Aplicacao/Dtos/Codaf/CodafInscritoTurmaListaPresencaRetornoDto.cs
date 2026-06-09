namespace SME.ConectaFormacao.Aplicacao.Dtos.Codaf
{
    public class CodafInscritoTurmaListaPresencaRetornoDto
    {
        public long Id { get; set; }
        public string Documento { get; set; } = null!;
        public string Nome { get; set; } = null!;
        public decimal? PercentualFrequencia { get; set; }
        public string? ConceitoFinal { get; set; }
        public bool? AtividadeObrigatorio { get; set; }
        public bool? Aprovado { get; set; }
    }
}