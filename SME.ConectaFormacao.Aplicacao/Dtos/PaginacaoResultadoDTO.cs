namespace SME.ConectaFormacao.Aplicacao.Dtos
{
    public class PaginacaoResultadoDTO<T>(IEnumerable<T> items, int totalRegistros, int numeroRegistros)
    {
        public IEnumerable<T> Items { get; set; } = items;
        public int TotalPaginas
        {
            get
            {
                return (int)Math.Ceiling((double)TotalRegistros / numeroRegistros);
            }
        }
        public int TotalRegistros { get; private set; } = totalRegistros;
    }
}
