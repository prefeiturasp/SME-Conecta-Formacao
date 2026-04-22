namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaEncontroDataDto
    {
        public long Id { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string? HoraInicio { get; set; }
        public string? HoraFim { get; set; }
    }
}
