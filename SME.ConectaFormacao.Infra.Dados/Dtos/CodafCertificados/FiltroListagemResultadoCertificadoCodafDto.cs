using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados
{
    public class FiltroListagemResultadoCertificadoCodafDto
    {
        public string? NumeroHomologacao { get; set; }
        public string? NomeFormacao { get; set; }
        public long? CodigoCertificado { get; set; }
        public TipoParticipacaoCodaf? TipoParticipacao { get; set; }
        public DateTime? DataEmissaoInicio { get; set; }
        public DateTime? DataEmissaoFim { get; set; }
        public required int Pagina { get; set; }
        public required int TamanhoPagina { get; set; }
    }
}
