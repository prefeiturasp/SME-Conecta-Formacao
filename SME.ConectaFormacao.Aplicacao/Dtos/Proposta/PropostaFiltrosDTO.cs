using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaFiltrosDTO
    {
        public long? AreaPromotoraId { get; set; }
        public Modalidade? Modalidade { get; set; }
        public long? PublicoAlvoId { get; set; }
        public string? NomeFormacao { get; set; }
        public long? NumeroHomologacao { get; set; }
        public DateTime? PeriodoRealizacaoInicio { get; set; }
        public DateTime? PeriodoRealizacaoFim { get; set; }
        public SituacaoProposta? Situacao { get; set; }
    }
}
