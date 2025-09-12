namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao
{
    public class InscricaoTransferenciaDTO
    {
        public long IdFormacaoOrigem { get; set; }
        public long IdTurmaOrigem { get; set; }
        public List<InscricaoTransferenciaDTOCursista> Cursistas { get; set; }
        public long IdFormacaoDestino { get; set; }
        public long IdTurmaDestino { get; set; }
    }
}
