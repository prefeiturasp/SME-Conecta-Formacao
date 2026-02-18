namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class UsuarioAcessibilidade : EntidadeBaseAuditavel
    {
        public long UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public bool PossuiDeficiencia { get; set; }
        public string? DescricaoDeficiencia { get; set; }
        public bool? NecessitaAdaptacao { get; set; }
        public string? DescricaoAdaptacao { get; set; }
    }
}