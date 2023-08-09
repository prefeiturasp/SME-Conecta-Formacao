namespace SME.ConectaFormacao.Infra.Servicos.Log
{
    public class LogMensagem
    {
        public LogMensagem(string mensagem, string contexto = "", string nivel = "", string observacao = "", string rastreamento = "", string projeto = "ConectaFormacao")
        {
            Mensagem = mensagem;
            Contexto = contexto;
            Nivel = nivel;
            Observacao = observacao;
            Rastreamento = rastreamento;
            Projeto = projeto;
        }

        public string Mensagem { get; }
        public string Contexto { get; }
        public string Nivel { get; }
        public string Observacao { get; }
        public string Rastreamento { get; }
        public string Projeto { get; }
    }
}
