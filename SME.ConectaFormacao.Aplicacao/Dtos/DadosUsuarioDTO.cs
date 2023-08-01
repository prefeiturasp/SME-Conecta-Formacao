using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.DTOS
{
    public class DadosUsuarioDTO
    {
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cep { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public int Tipo { get; set; }
    }
}
