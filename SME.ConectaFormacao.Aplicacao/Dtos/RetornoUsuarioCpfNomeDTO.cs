using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.Dtos
{
    public class RetornoUsuarioCpfNomeDTO
    {
        public string Cpf { get; set; }
        public string Nome { get; set; }
        public IEnumerable<DadosInscricaoCargoEol> UsuarioCargos { get; set; }
    }
}
