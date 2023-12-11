using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaPaginadaDTO
    {
        public long Id { get; set; }
        public string TipoFormacao { get; set; }
        public string AreaPromotora { get; set; }
        public string Formato { get; set; }
        public string NomeFormacao { get; set; }
        public long NumeroHomologacao { get; set; }
        public DateTime? PeriodoRealizacaoInicio { get; set; }
        public DateTime? PeriodoRealizacaoFim { get; set; }
        public DateTime? DataRealizacaoInicio { get; set; }
        public DateTime? DataRealizacaoFim { get; set; }
        public DateTime? DataInscricaoInicio { get; set; }
        public DateTime? DataInscricaoFim { get; set; }
        public string Situacao { get; set; }
        public FormacaoHomologada FormacaoHomologada { get; set; }
    }
}
