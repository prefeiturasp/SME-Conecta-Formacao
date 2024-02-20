using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaFiltrosDashboardDTO
    {
        public long? Id { get; set; }
        public long? AreaPromotoraId { get; set; }
        public Formato? Formato { get; set; }
        public long[]? PublicoAlvoIds { get; set; }
        public string? NomeFormacao { get; set; }
        public long? NumeroHomologacao { get; set; }
        public DateTime? PeriodoRealizacaoInicio { get; set; }
        public DateTime? PeriodoRealizacaoFim { get; set; }
        public SituacaoProposta? Situacao { get; set; }
        public bool? FormacaoHomologada { get; set; }
    }
}