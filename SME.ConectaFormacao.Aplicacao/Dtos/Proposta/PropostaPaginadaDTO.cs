namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaPaginadaDTO
    {
        public long Id { get; set; }
        public string TipoFormacao { get; set; }
        public string AreaPromotora { get; set; }
        public string Modalidade { get; set; }
        public string NomeFormacao { get; set; }
        public long NumeroHomologacao { get; set; }
        public DateTime? PeriodoRealizacaoInicio { get; set; }
        public DateTime? PeriodoRealizacaoFim { get; set; }
        public string Situacao { get; set; }
    }
}
