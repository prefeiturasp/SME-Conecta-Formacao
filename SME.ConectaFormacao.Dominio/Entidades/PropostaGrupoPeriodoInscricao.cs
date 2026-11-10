namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaGrupoPeriodoInscricao : PropostaGrupoPeriodoBase<PropostaGrupoPeriodoInscricaoTurma>
    {
        protected override PropostaGrupoPeriodoInscricaoTurma CriarVinculoTurma(long propostaTurmaId) =>
            new() { GrupoPeriodoInscricaoId = Id, PropostaTurmaId = propostaTurmaId };

        public bool EstaEmPeriodo(DateTime? dataReferencia = null)
        {
            var agora = dataReferencia ?? Extensoes.DateTimeExtension.HorarioBrasilia();
            var fim = DataFim.TimeOfDay == TimeSpan.Zero
                ? DataFim.Date.AddDays(1).AddMilliseconds(-1)
                : DataFim;

            return agora >= DataInicio && agora <= fim;
        }
    }
}
