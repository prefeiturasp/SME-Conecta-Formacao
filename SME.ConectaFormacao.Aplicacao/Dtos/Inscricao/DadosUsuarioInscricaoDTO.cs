namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao
{
    public class DadosInscricaoDTO
    {
        public string UsuarioNome { get; set; }
        public string UsuarioRf { get; set; }
        public string UsuarioCpf { get; set; }
        public string UsuarioEmail { get; set; }
        public IEnumerable<RetornoListagemDTO> UsuarioCargos { get; set; }
        public IEnumerable<RetornoListagemDTO> UsuarioFuncoes { get; set; }
    }
}
