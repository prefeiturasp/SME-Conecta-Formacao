namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaRegenteDTO
    {
        public long Id { get; set; }
        public bool ProfissionalRedeMunicipal { get; set; }
        public string? RegistroFuncional { get; set; }
        public string? NomeRegente { get; set; }
        public string? MiniBiografia { get; set; }
        public string? NomesTurmas { get; set; }
        public IEnumerable<PropostaRegenteTurmaDTO> Turmas { get; set; } = new List<PropostaRegenteTurmaDTO>();
    }
}