using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;

namespace SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares
{
    public class CodafSuplementarCadastroDto
    {
        public long CodafId { get; set; }
        public DateTime? DataPublicacao { get; set; }
        public DateTime? DataPublicacaoDom { get; set; }
        public short? NumeroComunicado { get; set; }
        public short? PaginaComunicadoDom { get; set; }
        public int? CodigoCursoEol { get; set; }
        public int? CodigoNivel { get; set; }
        public string? Observacao { get; set; }
        public IList<CodafSuplementarInscritoSalvarDto>? Inscritos { get; set; }
        public IList<CodafSuplementarRetificacaoSalvarDto>? Retificacoes { get; set; }
        public IList<CodafAnexoSalvarDto>? Anexos { get; set; }
    }
}
