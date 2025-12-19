namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CodafRetificacao : EntidadeBaseAuditavel
    {
        public int CodafListaPresencaId { get; set; }
        public CodafListaPresenca? CodafListaPresenca { get; set; }
        public DateOnly DataRetificacao { get; set; }
        public short PaginaRetificacaoDom { get; set; }
    }
}