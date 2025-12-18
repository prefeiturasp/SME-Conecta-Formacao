namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CodafComentario : EntidadeBaseAuditavel
    {
        public int CodafListaPresencaId { get; set; }
        public virtual CodafListaPresenca? CodafListaPresenca { get; set; }
        public required string Comentario { get; set; }
        public bool NotificacaoEnviada { get; set; }
        public DateTime? DataNotificacao { get; set; }
        public bool Ativo { get; set; }
    }
}