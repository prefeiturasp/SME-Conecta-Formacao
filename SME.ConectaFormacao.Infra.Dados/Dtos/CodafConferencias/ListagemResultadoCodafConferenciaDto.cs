using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafConferencias
{
    public class ListagemResultadoCodafConferenciaDto
    {
        public long Id { get; set; }
        public long NumeroHomologacao { get; set; }
        public string? NomeFormacao { get; set; }
        public long CodigoFormacao { get; set; }
        public required string NomeTurma { get; set; }
        public required string NomeAreaPromotora { get; set; }
        public StatusCodafConferencia Status { get; set; }
        public StatusCertificacaoTurma StatusCertificacaoTurma { get; set; }
        public int? CodigoCursoEol { get; set; }
        public int? CodigoNivel { get; set; }
        public bool PossuiAprovacoes { get; set; }
        public DateTime CriadoEm { get; set; }
    }
}
