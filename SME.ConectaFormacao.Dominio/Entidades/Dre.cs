namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class Dre : EntidadeBaseAuditavel
    {
        public string Codigo { get; set; }
        public string Abreviacao { get; set; }
        public string Nome { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public bool Todos { get; set; }
        public short Ordem { get; set; }
    }
}