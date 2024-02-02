using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class Usuario : EntidadeBaseAuditavel
    {
        public Usuario(string login, string nome, string email, string cpf, TipoUsuario tipo, SituacaoCadastroUsuario situacao, string? ueCodigo)
        {
            Login = login;
            Nome = nome;
            Email = email;
            Cpf = cpf;
            Tipo = tipo;
            Situacao = situacao;
            UeCodigo = ueCodigo;
        }

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
        public string Cpf { get; set; }
        public string? UeCodigo { get; set; }
        public TipoUsuario Tipo { get; set; }
        public bool PossuiContratoExterno { get; set; }
        public SituacaoCadastroUsuario Situacao { get; set; }

        public void Atualizar(string email, DateTime? dataHora, string? cpf)
        {
            Email = email;
            UltimoLogin = dataHora;
            if (!string.IsNullOrEmpty(cpf))
            {
                Cpf = cpf;
            }
        }

        public void Ativar()
        {
            Situacao = SituacaoCadastroUsuario.Ativo;
        }
    }
}