namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaParecerCompletoDTO
    {
        public long PropostaId { get; set; }
        
        public bool PodeInserir { get; set; }
        
        public IEnumerable<PropostaParecerDTO> Itens { get; set; } = Enumerable.Empty<PropostaParecerDTO>();
    }
}
