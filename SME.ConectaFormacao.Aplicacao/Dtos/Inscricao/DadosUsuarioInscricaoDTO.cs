namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao
{
    public class DadosUsuarioInscricaoDTO
    {
        public string Nome { get; set; }
        public string Rf { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public IEnumerable<RetornoListagemDTO> Cargos { get; set; }
        public IEnumerable<RetornoListagemDTO> Funcoes { get; set; }
    }
}
