using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Codaf
{
    public class ListaPresencaCodafResumoDto
    {
        public long Id { get; set; }
        public long NumeroHomologacao { get; set; }
        public string? NomeFormacao { get; set; }
        public long CodigoFormacao { get; set; }
        public required string NomeTurma { get; set; }
        public required string NomeAreaPromotora { get; set; }
        public StatusCodafListaPresenca Status { get; set; }
        public StatusCertificacaoTurma StatusCertificacaoTurma { get; set; }
    }
}
