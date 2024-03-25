using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Usuario
{
    public class DadosUsuarioDTO
    {
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string NomeUnidade { get; set; }
        public TipoUsuario Tipo { get; set; }
        public string EmailEducacional { get; set; }
    }
}
