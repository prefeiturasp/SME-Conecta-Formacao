
namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class ComponenteCurricular : EntidadeBaseAuditavel
    {
        public long? AnoTurmaId { get; set; }
        public long CodigoEOL { get; set; }
        public string Nome { get; set; }
        public bool Todos { get; set; }
        public short Ordem { get; set; }

        public AnoTurma AnoTurma { get; set; }
    }
}
