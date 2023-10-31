namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaTutorDTO
    {
        public long Id { get; set; }
        public bool ProfissionalRedeMunicipal { get; set; }
        public string? RegistroFuncional { get; set; }
        public string? NomeTutor { get; set; }
        public string? NomesTurmas { get; set; }
        public IEnumerable<PropostaTutorTurmaDTO> Turmas { get; set; } = new List<PropostaTutorTurmaDTO>();
    }
}