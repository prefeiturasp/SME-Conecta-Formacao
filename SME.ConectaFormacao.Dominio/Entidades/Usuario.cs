namespace SME.ConectaFormacao.Dominio
{
    public class Usuario : EntidadeBaseAuditavel
    {
        public Usuario() { }
        public Usuario(string login, string nome)
        {
            Login = login;
            Nome = nome;
        }

        public string Login { get; set; }
        public string Nome { get; set; }
        public DateTime? UltimoLogin { get; set; }
        public Guid? TokenRecuperacaoSenha { get; set; }
        public DateTime? ExpiracaoRecuperacaoSenha { get; set; }

        public void AtualizarUltimoLogin(DateTime? dataHora)
        {
            UltimoLogin = dataHora;
        }
    }
}