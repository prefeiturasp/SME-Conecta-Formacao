namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CodafSuplementarRetificacao : EntidadeBaseAuditavel
    {
        public long CodafSuplementarId { get; set; }
        public CodafSuplementar? CodafSuplementar { get; set; }
        public DateTime DataRetificacao { get; set; }
        public short PaginaRetificacaoDom { get; set; }

        public void AtualizarInformacoes(DateTime dataRetificacao, short paginaRetificacaoDom)
        {
            DataRetificacao = dataRetificacao;
            PaginaRetificacaoDom = paginaRetificacaoDom;
        }
    }
}