
namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class FormacaoTurma
    {
        public string Nome { get; set; }
        public string Local { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFim { get; set; }
        public long PropostaEncontroId { get; set; }
        public IEnumerable<FormacaoTurmaData> Periodos { get; set; }
    }
}
