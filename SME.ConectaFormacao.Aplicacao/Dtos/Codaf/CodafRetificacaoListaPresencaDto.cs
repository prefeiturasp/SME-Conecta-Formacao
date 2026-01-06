namespace SME.ConectaFormacao.Aplicacao.Dtos.Codaf
{
    public class CodafRetificacaoListaPresencaDto
    {
        public long Id { get; set; }
        public long CodafListaPresencaId { get; set; }
        public short PaginaRetificacaoDom { get; set; }
        public DateTime DataRetificacao { get; set; }
        public DateTime? AlteradoEm { get; set; }
        public string? AlteradoPor { get; set; }
        public string? AlteradoLogin { get; set; }
        public DateTime CriadoEm { get; set; }
        public string? CriadoPor { get; set; }
        public string? CriadoLogin { get; set; }
    }
}