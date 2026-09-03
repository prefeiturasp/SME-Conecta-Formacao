using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.CodafCursosNaoHomologados
{
    public class CodafCursoNaoHomologadoResumoDto
    {
        public long Id { get; set; }
        public long NumeroHomologacao { get; set; }
        public string? NomeFormacao { get; set; }
        public long CodigoFormacao { get; set; }
        public required string NomeTurma { get; set; }
        public required string NomeAreaPromotora { get; set; }
        public StatusCodafCursoNaoHomologado Status { get; set; }
        public StatusDeclaracaoTurma StatusDeclaracaoTurma { get; set; }
    }
}