namespace SME.ConectaFormacao.Aplicacao.Dtos
{
    public class FiltroListagemFormacaoDTO
    {
        public long[]? PublicosAlvosIds { get; set; }
        public string? Titulo { get; set; }
        public long[]? AreasPromotorasIds { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public int[]? FormatosIds { get; set; }
        public long[]? PalavrasChavesIds { get; set; }
    }
}
