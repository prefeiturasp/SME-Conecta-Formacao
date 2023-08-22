namespace SME.ConectaFormacao.Aplicacao.Dtos
{
    public class PaginacaoResultadoDTO<T>
    {
        public PaginacaoResultadoDTO(IEnumerable<T> items, int totalRegistros, int numeroRegistros)
        {
            Items = items;
            TotalRegistros = totalRegistros;
            NumeroRegistros = numeroRegistros;
        }

        public IEnumerable<T> Items { get; set; }
        public int TotalPaginas
        {
            get
            {
                return (int)Math.Ceiling((double)TotalRegistros / NumeroRegistros);
            }
        }
        public int TotalRegistros { get; private set; }
        public int NumeroRegistros { get; set; }
    }
}
