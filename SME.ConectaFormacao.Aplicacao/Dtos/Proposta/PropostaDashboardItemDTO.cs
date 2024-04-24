namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaDashboardItemDTO
    {
        public long Numero { get; set; }
        public string? Nome { get; set; }
        public string? Data { get; set; }
        public string? LinkParaInscricoesExterna { get; set; }
        public bool PodeEnviarInscricao { get; set; }
    }
}