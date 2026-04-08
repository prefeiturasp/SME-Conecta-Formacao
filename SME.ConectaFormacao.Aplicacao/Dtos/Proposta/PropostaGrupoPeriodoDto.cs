namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaGrupoPeriodoDto
    {
        public long Id { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public IEnumerable<long> PropostaTurmasIds { get; set; } = [];
    }
}
