using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Codaf
{
    public class FiltroListaMinhasDeclaracoesCodafDto
    {
        public string? CodigoHomologacao { get; set; }
        public string? NomeFormacao { get; set; }
        public long? CodigoDeclaracao { get; set; }
        public TipoParticipacaoCodaf? TipoParticipacao { get; set; }
        public DateTime? DataEmissaoInicio { get; set; }
        public DateTime? DataEmissaoFim { get; set; }
        public required int NumeroPagina { get; set; } = 1;
        public required int NumeroRegistros { get; set; } = 10;
    }
}