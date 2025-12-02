namespace SME.ConectaFormacao.Infra.Dados.Dtos
{
    public class PaginacaoResultadoDto<T>
    {
        public IEnumerable<T> Itens { get; set; } = [];
        public int TotalRegistros { get; set; }
        public int PaginaAtual { get; set; }
        public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / TamanhoPagina);
        public int TamanhoPagina { get; set; }
    }
}
