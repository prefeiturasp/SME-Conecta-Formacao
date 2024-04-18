namespace SME.ConectaFormacao.Aplicacao
{
    public class TurmasIdParaCancelamentoDTO
    {
        public TurmasIdParaCancelamentoDTO(long[] turmasIds)
        {
            TurmasIds = turmasIds;
        }

        public long[] TurmasIds { get; set; }
    }
}