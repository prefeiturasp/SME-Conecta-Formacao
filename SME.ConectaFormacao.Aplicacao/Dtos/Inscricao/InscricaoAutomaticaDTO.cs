namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao
{
    public class InscricaoAutomaticaDTO
    {
        public long PropostaId { get; set; }
        public long PropostaTurmaId { get; set; }

        public long? CargoId { get; set; }
        public string? CargoCodigo { get; set; }
        public string? CargoDreCodigo { get; set; }
        public string? CargoUeCodigo { get; set; }

        public long? FuncaoId { get; set; }
        public string? FuncaoCodigo { get; set; }
        public string? FuncaoDreCodigo { get; set; }
        public string? FuncaoUeCodigo { get; set; }
        
        public long UsuarioId { get; set; }
        public bool EhFormacaoHomologada { get; set; }
        public string UsuarioRf { get; set; }
        public string UsuarioNome { get; set; }
    }
}
