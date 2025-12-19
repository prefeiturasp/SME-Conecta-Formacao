namespace SME.ConectaFormacao.Aplicacao.Dtos.Codaf
{
    public class CodafListaPresencaCadastroDto
    {
        public long PropostaId { get; set; }
        public long PropostaTurmaId { get; set; }
        public DateOnly? DataPublicacao { get; set; }
        public DateOnly? DataPublicacaoDom { get; set; }
        public short? NumeroComunicado { get; set; }
        public short? PaginaComunicadoDom { get; set; }
        public int? CodigoCursoEol { get; set; }
        public int? CodigoNivel { get; set; }
        public string? Observacao { get; set; }
    }
}