namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaTurmaDre : EntidadeBaseAuditavel
    {
        public long PropostaTurmaId { get; set; }
        public long? DreId { get; set; }
        public string DreCodigo { get; set; }
        public Dre Dre { get; set; }
    }
}
