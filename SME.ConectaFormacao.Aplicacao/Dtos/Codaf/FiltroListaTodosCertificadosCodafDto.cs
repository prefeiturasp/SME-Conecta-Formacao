using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Codaf
{
    public class FiltroListaTodosCertificadosCodafDto
    {
        public string? NumeroHomologacao { get; set; }
        public string? NomeFormacao { get; set; }
        public long? CodigoCertificado { get; set; }
        public TipoCertificadoCodaf? TipoCertificado { get; set; }
        public string? DocumentoCursista { get; set; }
        public string? DocumentoRegente { get; set; }
        public string? NomeCursista { get; set; }
        public DateTime? DataEmissao { get; set; }
        public long? DreId { get; set; }
        public required int NumeroPagina { get; set; } = 1;
        public required int NumeroRegistros { get; set; } = 10;
    }
}