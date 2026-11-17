using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;

namespace SME.ConectaFormacao.Aplicacao.Dtos.CodafConferencias
{
    public class CodafConferenciaCadastroDto
    {
        public long PropostaId { get; set; }
        public long PropostaTurmaId { get; set; }
        public short? NumeroComunicado { get; set; }
        public DateTime? DataComunicado { get; set; }
        public short? PaginaComunicadoDom { get; set; }
        public DateTime? DataPublicacaoDom { get; set; }
        public int? CodigoCursoEol { get; set; }
        public int? CodigoNivel { get; set; }
        public string? Observacao { get; set; }
        public IList<CodafAnexoSalvarDto>? Anexos { get; set; }
        public IList<CodafConferenciaInscritoSalvarDto>? Inscritos { get; set; }
    }
}
