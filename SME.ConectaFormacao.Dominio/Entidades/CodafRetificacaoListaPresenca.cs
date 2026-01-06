namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CodafRetificacaoListaPresenca : EntidadeBaseAuditavel
    {
        public long CodafListaPresencaId { get; set; }
        public CodafListaPresenca? CodafListaPresenca { get; set; }
        public DateTime DataRetificacao { get; set; }
        public short PaginaRetificacaoDom { get; set; }

        public void AtualizarInformacoes(DateTime dataRetificacao, short paginaRetificacaoDom)
        {
            DataRetificacao = dataRetificacao;
            PaginaRetificacaoDom = paginaRetificacaoDom;
        }
    }
}