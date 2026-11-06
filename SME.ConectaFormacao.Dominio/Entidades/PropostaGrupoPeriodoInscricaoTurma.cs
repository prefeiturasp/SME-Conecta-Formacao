namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaGrupoPeriodoInscricaoTurma : PropostaGrupoPeriodoTurmaBase
    {
        public long GrupoPeriodoInscricaoId { get; set; }
        public PropostaGrupoPeriodoInscricao GrupoPeriodoInscricao { get; set; } = null!;
    }
}
