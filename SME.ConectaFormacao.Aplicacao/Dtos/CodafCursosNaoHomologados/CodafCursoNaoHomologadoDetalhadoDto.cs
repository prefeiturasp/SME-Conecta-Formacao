using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.CodafCursosNaoHomologados
{
    public class CodafCursoNaoHomologadoDetalhadoDto
    {
        public long Id { get; set; }
        public long PropostaId { get; set; }
        public long PropostaTurmaId { get; set; }
        public string? Observacao { get; set; }
        public StatusCodafCursoNaoHomologado Status { get; set; }
        public DateTime? AlteradoEm { get; set; }
        public string? AlteradoPor { get; set; }
        public string? AlteradoLogin { get; set; }
        public DateTime CriadoEm { get; set; }
        public string? CriadoPor { get; set; }
        public string? CriadoLogin { get; set; }
        public IList<CodafCursoNaoHomologadoAnexoDto>? Anexos { get; set; }
        public IList<CodafCursoNaoHomologadoInscritoDto>? Inscritos { get; set; }
        public PropostaTurmaComCodafDto? PropostaTurma { get; set; }
    }
}