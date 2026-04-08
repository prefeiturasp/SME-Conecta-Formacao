namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaEncontroData : EntidadeBaseAuditavel
    {
        public long PropostaEncontroId { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string? HoraInicio { get; set; }
        public string? HoraFim { get; set; }
        public bool HouveAlteracao(PropostaEncontroData outraData)
        {
            return DataInicio != outraData.DataInicio ||
                   DataFim != outraData.DataFim ||
                   HoraInicio != outraData.HoraInicio ||
                   HoraFim != outraData.HoraFim;
        }

        public bool SemDataFim()
        {
            return !DataFim.HasValue || DataInicio == DataFim;
        }
    }
}
