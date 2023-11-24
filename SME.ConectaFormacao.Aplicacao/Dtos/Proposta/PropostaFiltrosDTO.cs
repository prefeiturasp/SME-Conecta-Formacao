using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaFiltrosDTO
    {
        public long? Id { get; set; }
        public long? AreaPromotoraId { get; set; }
        public Modalidade? Modalidade { get; set; }
        public long[]? PublicoAlvoIds { get; set; }
        public string? NomeFormacao { get; set; }
        public long? NumeroHomologacao { get; set; }
        public DateTime? PeriodoRealizacaoInicio { get; set; }
        public DateTime? PeriodoRealizacaoFim { get; set; }
        public SituacaoProposta? Situacao { get; set; }
        public Guid? GrupoId { get; set; }
        public IEnumerable<string>? DresCodigo { get; set; }
    }
}
