namespace SME.ConectaFormacao.Aplicacao.Dtos
{
    public class PaginacaoResultadoComMensagemDTO<T> : PaginacaoResultadoDTO<T>
    {
        public PaginacaoResultadoComMensagemDTO(IEnumerable<T> items, int totalRegistros, int numeroRegistros, bool sucesso, string mensagem = "") 
            : base(items, totalRegistros, numeroRegistros)
        {
            Sucesso = sucesso;
            Mensagem = mensagem;
        }

        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }
}
