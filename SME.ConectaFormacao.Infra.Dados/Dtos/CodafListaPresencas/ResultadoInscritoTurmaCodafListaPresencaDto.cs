namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas
{
    public class ResultadoInscritoTurmaCodafListaPresencaDto
    {
        public long Id { get; set; }
        public string Login { get; set; } = null!;
        public string Cpf { get; set; } = null!;
        public string Nome { get; set; } = null!;
        public decimal? PercentualFrequencia { get; set; }
        public string? ConceitoFinal { get; set; }
        public bool? AtividadeObrigatorio { get; set; }
        public bool? Aprovado { get; set; }
    }
}