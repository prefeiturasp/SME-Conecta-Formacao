using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.Dtos.FuncionarioExterno
{
    public class FuncionarioExternoDTO
    {
        public FuncionarioExternoDTO(string nomePessoa, string cpf, string codigoUe, string nomeUe, IEnumerable<RetornoListagemDTO> ues)
        {
            NomePessoa = nomePessoa;
            Cpf = cpf;
            CodigoUE = codigoUe;
            NomeUe = nomeUe;
            Ues = ues;
        }

        public string NomePessoa { get; set; }

        public string Cpf { get; set; }
        public string CodigoUE { get; set; }
        public string NomeUe { get; set; }
        public IEnumerable<RetornoListagemDTO> Ues { get; set; }
    }
}