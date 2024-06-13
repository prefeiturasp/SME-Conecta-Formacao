using MediatR;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Comandos.SalvarLogViaRabbit;

public class SalvarLogViaRabbitCommand : IRequest<bool>
{
    public SalvarLogViaRabbitCommand(string mensagem, LogNivel nivel, LogContexto contexto, string observacao = "", string projeto = "ConectaFormacao", string rastreamento = "", string excecaoInterna = "", string innerException = "")
    {
        Mensagem = mensagem;
        Nivel = nivel;
        Contexto = contexto;
        Observacao = observacao;
        Projeto = projeto;
        Rastreamento = rastreamento;
        ExcecaoInterna = excecaoInterna;
        InnerException = innerException;
    }

    public string Mensagem { get; set; }
    public LogNivel Nivel { get; set; }
    public LogContexto Contexto { get; set; }
    public string Observacao { get; set; }
    public string Projeto { get; set; }
    public string Rastreamento { get; set; }
    public string ExcecaoInterna { get; }
    public string InnerException { get; set; }
}