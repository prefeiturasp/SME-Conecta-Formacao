namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaPareceristaResumidoDTO
    {
        public string Login { get; set; }
        public string Nome { get; set; }

        public PropostaPareceristaResumidoDTO(string login, string nome)
        {
            Login = login;
            Nome = nome;
        }
    }
}