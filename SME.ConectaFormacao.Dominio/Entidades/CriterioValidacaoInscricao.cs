namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CriterioValidacaoInscricao : EntidadeBaseAuditavel
    {
        public string Nome { get; set; }
        public bool Unico { get; set; }
        public bool Outros { get; set; }
        public short Ordem { get; set; }
    }
}
