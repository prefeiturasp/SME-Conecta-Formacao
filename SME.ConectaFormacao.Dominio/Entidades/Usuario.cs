namespace SME.ConectaFormacao.Dominio
{
    public class Usuario : EntidadeBaseAuditavel
    {
        public Usuario() { }
        public Usuario(string login, string nome, string email)
        {
            Login = login;
            Nome = nome;
            Email = email;
        }

        public string Login { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public DateTime? UltimoLogin { get; set; }
        public Guid? TokenRecuperacaoSenha { get; set; }
        public DateTime? ExpiracaoRecuperacaoSenha { get; set; }

        public void Atualizar(string email, DateTime? dataHora)
        {
            Email = email;
            UltimoLogin = dataHora;
        }
    }
}