using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.CodafConferencias
{
    public class CodafConferenciaDetalhadoDto
    {
        public long Id { get; set; }
        public long PropostaId { get; set; }
        public long PropostaTurmaId { get; set; }
        public short? NumeroComunicado { get; set; }
        public DateTime? DataComunicado { get; set; }
        public short? PaginaComunicadoDom { get; set; }
        public DateTime? DataPublicacaoDom { get; set; }
        public int? CodigoCursoEol { get; set; }
        public int? CodigoNivel { get; set; }
        public string? Observacao { get; set; }
        public StatusCodafConferencia Status { get; set; }
        public DateTime? DataFinalizacao { get; set; }
        public DateTime? AlteradoEm { get; set; }
        public string? AlteradoPor { get; set; }
        public string? AlteradoLogin { get; set; }
        public DateTime CriadoEm { get; set; }
        public string? CriadoPor { get; set; }
        public string? CriadoLogin { get; set; }
        public string? NomeFormacao { get; set; }
        public long? CodigoFormacao { get; set; }
        public long? NumeroHomologacao { get; set; }
        public bool CertificadoEmitido { get; set; }
        public IList<CodafConferenciaAnexoDto>? Anexos { get; set; }
        public IList<CodafConferenciaInscritoDto>? Inscritos { get; set; }
        public PropostaTurmaComCodafDto? Turma { get; set; }
    }
}
