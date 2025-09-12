using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao
{
    public class RetornoInscricaoDTO
    {
        public int Status { get; set; }
        public string Mensagem { get; set; }
        public List<CursistaDTO> Cursistas { get; set; }

        public RetornoInscricaoDTO()
        {
            Cursistas = new List<CursistaDTO>();
        }
        public RetornoInscricaoDTO(string mensagem, List<CursistaDTO> cursistas)
        {
            Mensagem = mensagem;
            Cursistas = cursistas;
        }
    }
}
