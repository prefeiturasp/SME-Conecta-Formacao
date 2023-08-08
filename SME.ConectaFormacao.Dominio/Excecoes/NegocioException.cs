namespace SME.ConectaFormacao.Dominio.Excecoes;

public class NegocioException : Exception
{
    public NegocioException(string mensagem, int statusCode = 400) : base(mensagem)
    {
        Mensagens = new List<string>();
        StatusCode = statusCode;
    }

    public NegocioException(List<string> mensagens, int statusCode = 400)
    {
        Mensagens = mensagens;
        StatusCode = statusCode;
    }

    public int StatusCode { get; }
    public List<string> Mensagens { get; }
}