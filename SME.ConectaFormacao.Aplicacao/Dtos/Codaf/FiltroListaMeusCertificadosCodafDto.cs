using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Codaf
{
    public class FiltroListaMeusCertificadosCodafDto
    {
        public string? NumeroHomologacao { get; set; }
        public string? NomeFormacao { get; set; }
        public long? CodigoCertificado { get; set; }
        public TipoParticipacaoCodaf? TipoParticipacao { get; set; }
        public DateTime? DataEmissaoInicio { get; set; }
        public DateTime? DataEmissaoFim { get; set; }
        public required int NumeroPagina { get; set; } = 1;
        public required int NumeroRegistros { get; set; } = 10;
    }
}