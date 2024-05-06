using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class Usuario : EntidadeBaseAuditavel
    {
        public Usuario(string login, string nome, string email, string cpf, TipoUsuario tipo, SituacaoCadastroUsuario situacao, string? codigoEolUnidade, string? emailEducacional,TipoEmail tipoEmail)
        {
            Login = login;
            Nome = nome;
            Email = email;
            Cpf = cpf;
            Tipo = tipo;
            Situacao = situacao;
            CodigoEolUnidade = codigoEolUnidade;
            EmailEducacional = emailEducacional;
            TipoEmail = tipoEmail;
        }

        public Usuario() { }
        public Usuario(string login, string nome, string email)
        {
            Login = login;
            Nome = nome;
            Email = email;
            Tipo = TipoUsuario.Interno;
            Situacao = SituacaoCadastroUsuario.Ativo;
        }

        public string Login { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public DateTime? UltimoLogin { get; set; }
        public Guid? TokenRecuperacaoSenha { get; set; }
        public DateTime? ExpiracaoRecuperacaoSenha { get; set; }
        public string Cpf { get; set; }
        public string? CodigoEolUnidade { get; set; }
        public TipoUsuario Tipo { get; set; }
        public bool PossuiContratoExterno { get; set; }
        public SituacaoCadastroUsuario Situacao { get; set; }
        public string? EmailEducacional { get; set; }
        public TipoEmail? TipoEmail { get; set; }

        public void Atualizar(string email, DateTime? dataHora, string? cpf)
        {
            Email = email;
            UltimoLogin = dataHora;
            if (!string.IsNullOrEmpty(cpf))
            {
                Cpf = cpf;
            }

            TipoEmail ??= Enumerados.TipoEmail.FuncionarioUnidadeParceira;
        }

        public void Ativar()
        {
            Situacao = SituacaoCadastroUsuario.Ativo;
        }

        public bool EstaAguardandoValidacaoEmail()
        {
            return Situacao == SituacaoCadastroUsuario.AguardandoValidacaoEmail;
        }
    }
}