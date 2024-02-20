using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaEncontro : EntidadeBaseAuditavel
    {
        public long PropostaId { get; set; }
        public string? HoraInicio { get; set; }
        public string? HoraFim { get; set; }
        public TipoEncontro? Tipo { get; set; }
        public string Local { get; set; }

        public IEnumerable<PropostaEncontroTurma> Turmas { get; set; }
        public IEnumerable<PropostaEncontroData> Datas { get; set; }
    }
}
