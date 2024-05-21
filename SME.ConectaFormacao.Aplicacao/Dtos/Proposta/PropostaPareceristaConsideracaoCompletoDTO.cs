namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaPareceristaConsideracaoCompletoDTO
    {
        public long PropostaId { get; set; }

        public bool PodeInserir { get; set; }

        public IEnumerable<PropostaPareceristaConsideracaoDTO> Itens { get; set; } = Enumerable.Empty<PropostaPareceristaConsideracaoDTO>();
    }
}
