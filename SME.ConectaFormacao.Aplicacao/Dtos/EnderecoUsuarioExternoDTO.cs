namespace SME.ConectaFormacao.Aplicacao.DTOS
{
    public class EnderecoUsuarioExternoDTO
    {
        public string Endereco { get; set; }
        public string? Complemento { get; set; }
        public int Numero { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Cep { get; set; }
        public string Bairro { get; set; }
    }
}
