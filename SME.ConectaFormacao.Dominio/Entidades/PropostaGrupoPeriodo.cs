namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaGrupoPeriodo : PropostaGrupoPeriodoBase<PropostaGrupoPeriodoTurma>
    {
        protected override PropostaGrupoPeriodoTurma CriarVinculoTurma(long propostaTurmaId) =>
            new() { GrupoPeriodoId = Id, PropostaTurmaId = propostaTurmaId };
    }
}
