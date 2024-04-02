namespace SME.ConectaFormacao.Aplicacao.Dtos
{
    public class PaginacaoResultadoComMensagemDTO<T> : PaginacaoResultadoDTO<T>
    {
        public PaginacaoResultadoComMensagemDTO(IEnumerable<T> items, int totalRegistros, int numeroRegistros, bool podeInscrever, string mensagem) 
            : base(items, totalRegistros, numeroRegistros)
        {
            PodeInscrever = podeInscrever;
            Mensagem = mensagem;
        }

        public bool PodeInscrever { get; set; }
        public string Mensagem { get; set; }
    }
}
