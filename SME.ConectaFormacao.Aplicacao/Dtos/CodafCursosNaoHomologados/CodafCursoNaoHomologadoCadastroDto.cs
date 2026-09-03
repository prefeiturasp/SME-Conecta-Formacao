using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;

namespace SME.ConectaFormacao.Aplicacao.Dtos.CodafCursosNaoHomologados
{
    public class CodafCursoNaoHomologadoCadastroDto
    {
        public long PropostaId { get; set; }
        public long PropostaTurmaId { get; set; }
        public string? Observacao { get; set; }
        public IList<CodafAnexoSalvarDto>? Anexos { get; set; }
        public IList<CodafCursoNaoHomologadoInscritoSalvarDto>? Inscritos { get; set; }
    }
}