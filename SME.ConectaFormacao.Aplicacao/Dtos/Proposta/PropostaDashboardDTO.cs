using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaDashboardDTO
    {
        public PropostaDashboardDTO()
        {
            Propostas = new List<PropostaDashboardItemDTO>();
        }
        public SituacaoProposta Situacao { get; set; }
        public string? Cor { get; set; }
        public string TotalRegistros { get; set; }
        public List<PropostaDashboardItemDTO> Propostas { get; set; }
    }
}