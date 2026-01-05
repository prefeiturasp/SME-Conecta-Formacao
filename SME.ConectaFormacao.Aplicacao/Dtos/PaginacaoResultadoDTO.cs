namespace SME.ConectaFormacao.Aplicacao.Dtos
{
    public class PaginacaoResultadoDto<T>
    {
        private readonly int numeroRegistros;

        public IEnumerable<T> Items { get; set; }
        public int TotalPaginas
        {
            get
            {
                return numeroRegistros > 0 ? (int)Math.Ceiling((double)TotalRegistros / numeroRegistros) : 0;
            }
        }
        public int TotalRegistros { get; private set; }

        public PaginacaoResultadoDto(IEnumerable<T> items, int totalRegistros, int numeroRegistros)
        {
            this.numeroRegistros = numeroRegistros;
            Items = items;
            TotalRegistros = totalRegistros;
        }

        protected PaginacaoResultadoDto()
        {
            Items = [];
        }
    }
}
