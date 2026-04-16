namespace SME.ConectaFormacao.Dominio.ObjetosDeValor
{
    public class FormacaoTurmaDataNovo
    {
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public long PropostaEncontroId { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFim { get; set; }
    }
}
