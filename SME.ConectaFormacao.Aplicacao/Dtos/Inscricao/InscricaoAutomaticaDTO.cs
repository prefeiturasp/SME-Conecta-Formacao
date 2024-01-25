namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao
{
    public class InscricaoAutomaticaDTO
    {
        public long PropostaId { get; set; }
        public long PropostaTurmaId { get; set; }
        public long? CargoId { get; set; }
        public string? CargoCodigo { get; set; }
        public long? FuncaoId { get; set; }
        public string? FuncaoCodigo { get; set; }
        public long UsuarioId { get; set; }
        public string UsuarioRf { get; set; }
        public string UsuarioNome { get; set; }
        public string UsuarioCpf { get; set; }
    }
}
