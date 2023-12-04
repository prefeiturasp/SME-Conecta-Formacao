
namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class ComponenteCurricular : EntidadeBaseAuditavel
    {
        public long AnoId { get; set; }
        public long CodigoEOL { get; set; }
        public string nome { get; set; }
        
        public Ano Ano { get; set; }
    }
}
