namespace SME.ConectaFormacao.Aplicacao.Dtos.Usuario
{
    public class UsuarioAcessibilidadeDto
    {
        public long Id { get; set; }
        public long UsuarioId { get; set; }
        public bool? PossuiDeficiencia { get; set; }
        public string? DescricaoDeficiencia { get; set; }
        public bool? NecessitaAdaptacao { get; set; }
        public string? DescricaoAdaptacao { get; set; }
        public bool Salvar { get; set; }
    }
}