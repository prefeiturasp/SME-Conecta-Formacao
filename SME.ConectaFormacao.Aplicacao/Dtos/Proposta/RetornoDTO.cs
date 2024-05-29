namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class RetornoDTO
    {
        public bool Sucesso { get; set; }
        public long EntidadeId { get; set; }
        public string Mensagem { get; set; }

        public static RetornoDTO RetornarSucesso(string mensagem, long id)
        {
            return new RetornoDTO
            {
                Sucesso = true,
                Mensagem = mensagem,
                EntidadeId = id
            };
        }

        public static RetornoDTO RetornarSucesso(string mensagem)
        {
            return new RetornoDTO
            {
                Sucesso = true,
                Mensagem = mensagem
            };
        }
    }
}
