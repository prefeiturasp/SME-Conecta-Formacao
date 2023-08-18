using System.Net;

namespace SME.ConectaFormacao.Dominio.Excecoes;

public class NegocioException : Exception
{
    public NegocioException(string mensagem, HttpStatusCode statusCode = HttpStatusCode.BadRequest) : base(mensagem)
    {
        Mensagens = new List<string>() { mensagem };
        StatusCode = (int)statusCode;
    }

    public NegocioException(List<string> mensagens, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        Mensagens = mensagens;
        StatusCode = (int)statusCode;
    }

    public int StatusCode { get; }
    public List<string> Mensagens { get; }
}