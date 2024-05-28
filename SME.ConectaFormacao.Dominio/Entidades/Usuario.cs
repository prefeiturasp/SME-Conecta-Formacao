using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class Usuario : EntidadeBaseAuditavel
    {
        public Usuario(string login, string nome, string email, string cpf, TipoUsuario tipo, SituacaoUsuario situacao, string? codigoEolUnidade, string? emailEducacional, TipoEmail tipoEmail)
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
            Situacao = SituacaoUsuario.Ativo;
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
        public SituacaoUsuario Situacao { get; set; }
        public string? EmailEducacional { get; set; }
        public TipoEmail? TipoEmail { get; set; }
        public long? AreaPromotoraId { get; set; }
        public string? Telefone { get; set; }

        public AreaPromotora AreaPromotora { get; set; }

        public void Atualizar(string email, DateTime? dataHora, string? cpf)
        {
            //TODO: Evitar a atualização do e-mail ao logar
            // Email = email;
            UltimoLogin = dataHora;
            if (!string.IsNullOrEmpty(cpf))
            {
                Cpf = cpf;
            }

            TipoEmail ??= Enumerados.TipoEmail.FuncionarioUnidadeParceira;
        }

        public void Ativar()
        {
            Situacao = SituacaoUsuario.Ativo;
        }

        public bool EstaAguardandoValidacaoEmail()
        {
            return Situacao == SituacaoUsuario.AguardandoValidacaoEmail;
        }
    }
}