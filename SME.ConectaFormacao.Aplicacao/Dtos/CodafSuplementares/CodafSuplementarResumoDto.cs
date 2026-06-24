using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares
{
    public class CodafSuplementarResumoDto
    {
        public long Id { get; set; }
        public long NumeroHomologacao { get; set; }
        public string? NomeFormacao { get; set; }
        public long CodigoFormacao { get; set; }
        public required string NomeTurma { get; set; }
        public required string NomeAreaPromotora { get; set; }
        public StatusCodafSuplementar Status { get; set; }
        public StatusCertificacaoTurma StatusCertificacaoTurma { get; set; }
        public string? CodigoCursoEol { get; set; }
        public string? CodigoNivel { get; set; }
    }
}
