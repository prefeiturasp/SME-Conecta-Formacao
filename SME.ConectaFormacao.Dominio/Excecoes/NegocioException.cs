using System.Net;

namespace SME.ConectaFormacao.Dominio.Excecoes;

public class NegocioException : Exception
{
    public NegocioException(string mensagem, int statusCode = 601) : base(mensagem)
    {
        Mensagens = new List<string>();
        StatusCode = statusCode;
    }

    public NegocioException(List<string> mensagens, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        Mensagens = mensagens;
        StatusCode = (int)statusCode;
    }

    public NegocioException(string mensagen, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        Mensagens = new List<string>() { mensagen };
        StatusCode = (int)statusCode;
    }

    public int StatusCode { get; }
    public List<string> Mensagens { get; }
}