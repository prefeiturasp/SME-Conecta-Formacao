namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaGrupoPeriodoTurma : PropostaGrupoPeriodoTurmaBase
    {
        public long GrupoPeriodoId { get; set; }
        public PropostaGrupoPeriodo GrupoPeriodo { get; set; } = null!;
    }
}
