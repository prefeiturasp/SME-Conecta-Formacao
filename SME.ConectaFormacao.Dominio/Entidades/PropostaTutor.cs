namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaTutor : EntidadeBaseAuditavel
    {
        public long PropostaId { get; set; }
        public bool ProfissionalRedeMunicipal { get; set; }
        public string? RegistroFuncional { get; set; }
        public string? NomeTutor { get; set; }
        public IEnumerable<PropostaTutorTurma> Turmas { get; set; } = new List<PropostaTutorTurma>();
    }
}