namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaDashboardDTO
    {
        public PropostaDashboardDTO()
        {
            Propostas = new List<PropostaDashboardItemDTO>();
        }
        public string? Situacao { get; set; }
        public string? Cor { get; set; }
        public bool VerMais { get; set; }
        public int TotalRegistros { get; set; }
        public List<PropostaDashboardItemDTO> Propostas { get; set; }
    }
}