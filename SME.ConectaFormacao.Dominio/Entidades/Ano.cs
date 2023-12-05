
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class Ano : EntidadeBaseAuditavel
    {
        public string CodigoEOL { get; set; }
        public string Descricao { get; set; }
        public long CodigoSerieEnsino { get; set; }
        public Modalidade Modalidade { get; set; }
        public bool Todos { get; set; }
        public int Ordem { get; set; }
        public int AnoLetivo { get; set; }
    }
}
