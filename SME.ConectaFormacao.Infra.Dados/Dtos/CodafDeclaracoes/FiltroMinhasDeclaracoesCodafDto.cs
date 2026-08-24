using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafDeclaracoes
{
    public class FiltroMinhasDeclaracoesCodafDto
    {
        public string? CodigoFormacao { get; set; }
        public string? NomeFormacao { get; set; }
        public long? CodigoDeclaracao { get; set; }
        public TipoParticipacaoCodaf? TipoParticipacao { get; set; }
        public DateTime? DataEmissaoInicio { get; set; }
        public DateTime? DataEmissaoFim { get; set; }
        public required int Pagina { get; set; }
        public required int TamanhoPagina { get; set; }
    }
}
