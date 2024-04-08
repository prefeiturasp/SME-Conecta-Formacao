namespace SME.ConectaFormacao.Aplicacao.Dtos
{
    public class PaginacaoResultadoComSucessoDTO<T> : PaginacaoResultadoDTO<T>
    {
        public PaginacaoResultadoComSucessoDTO(IEnumerable<T> items, int totalRegistros, int numeroRegistros, bool sucesso) 
            : base(items, totalRegistros, numeroRegistros)
        {
            Sucesso = sucesso;
        }
        public bool Sucesso { get; set; }
    }
}