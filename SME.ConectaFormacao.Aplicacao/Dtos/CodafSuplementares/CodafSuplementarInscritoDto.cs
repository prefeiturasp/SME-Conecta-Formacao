namespace SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares
{
    public class CodafSuplementarInscritoDto
    {
        public long Id { get; set; }
        public long InscricaoId { get; set; }
        public long CodafSuplementarId { get; set; }
        public string Documento { get; set; } = null!;
        public string Nome { get; set; } = null!;
        public decimal? PercentualFrequencia { get; set; }
        public string? ConceitoFinal { get; set; }
        public bool? AtividadeObrigatorio { get; set; }
        public bool? Aprovado { get; set; }
    }
}
