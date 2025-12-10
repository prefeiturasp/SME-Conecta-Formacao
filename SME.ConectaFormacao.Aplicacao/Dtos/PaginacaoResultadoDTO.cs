namespace SME.ConectaFormacao.Aplicacao.Dtos
{
    public class PaginacaoResultadoDto<T>(IEnumerable<T> items, int totalRegistros, int numeroRegistros)
    {
        public IEnumerable<T> Items { get; set; } = items;
        public int TotalPaginas
        {
            get
            {
                return numeroRegistros > 0 ? (int)Math.Ceiling((double)TotalRegistros / numeroRegistros) : 0;
            }
        }
        public int TotalRegistros { get; private set; } = totalRegistros;
    }
}
