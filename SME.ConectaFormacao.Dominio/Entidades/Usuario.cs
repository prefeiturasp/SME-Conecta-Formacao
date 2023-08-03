namespace SME.ConectaFormacao.Dominio
{
    public class Usuario : EntidadeBaseAuditavel
    {
        public string Login { get; set; }
        public string Nome { get; set; }
        public DateTime? UltimoLogin { get; set; }
        public Guid? TokenRecuperacaoSenha { get; set; }
        public DateTime? ExpiracaoRecuperacaoSenha { get; set; }
    }
}