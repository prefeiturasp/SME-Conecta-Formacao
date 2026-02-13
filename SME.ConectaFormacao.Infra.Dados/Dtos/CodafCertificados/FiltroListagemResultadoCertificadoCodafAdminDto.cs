using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados
{
    public class FiltroListagemResultadoCertificadoCodafAdminDto
    {
        public string? CodigoFormacao { get; set; }
        public string? NumeroHomologacao { get; set; }
        public string? NomeFormacao { get; set; }
        public long? PropostaTurmaId { get; set; }
        public string? CodigoCertificado { get; set; }
        public TipoCertificadoCodaf TipoCertificado { get; set; }
        public string? DocumentoCursista { get; set; }
        public string? DocumentoRegente { get; set; }
        public string? NomeRegente { get; set; }
        public DateTime? DataEmissao { get; set; }
        public long? DreId { get; set; }
        public required int Pagina { get; set; }
        public required int TamanhoPagina { get; set; }
    }
}
