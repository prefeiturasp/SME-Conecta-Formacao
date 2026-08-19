#pragma warning disable CS8618
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares
{
    public class ListagemResultadoCodafSuplementarDto
    {
        public long Id { get; set; }
        public long NumeroHomologacao { get; set; }
        public string? NomeFormacao { get; set; }
        public long CodigoFormacao { get; set; }
        public required string NomeTurma { get; set; }
        public required string NomeAreaPromotora { get; set; }
        public StatusCodafListaPresenca Status { get; set; }
        public StatusCertificacaoTurma StatusCertificacaoTurma { get; set; }
        public string? CodigoCursoEol { get; set; }
        public string? CodigoNivel { get; set; }
        public bool PossuiAprovacoes { get; set; }
    }
}
