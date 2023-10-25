namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaRegente : EntidadeBaseAuditavel
    {
        public long PropostaId { get; set; }
        public bool ProfissionalRedeMunicipal { get; set; }
        public string? RegistroFuncional { get; set; }
        public string NomeRegente { get; set; }
        public string MiniBiografia { get; set; }
        public IEnumerable<PropostaRegenteTurma> Turmas { get; set; } = new List<PropostaRegenteTurma>();
    }
}