namespace SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares
{
    public class CodafSuplementarInscritoSalvarDto
    {
        public long InscricaoId { get; set; }
        public decimal? PercentualFrequencia { get; set; }
        public string? ConceitoFinal { get; set; }
        public bool? AtividadeObrigatorio { get; set; }
        public bool? Aprovado { get; set; }
    }
}
