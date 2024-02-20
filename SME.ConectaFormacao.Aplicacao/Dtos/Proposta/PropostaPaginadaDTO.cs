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
        public string DataRealizacaoInicio { get; set; }
        public string DataRealizacaoFim { get; set; }
        public string Situacao { get; set; }
        public FormacaoHomologada FormacaoHomologada { get; set; }
    }
}
