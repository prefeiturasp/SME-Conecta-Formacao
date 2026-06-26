using MediatR;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Comandos.SalvarLog;

public class SalvarLogCommand : IRequest<bool>
{
    public SalvarLogCommand(string entidade, LogNivel nivelLog, string mensagem, string? complemento)
    {
        Entidade = entidade;
        NivelLog = nivelLog;
        Mensagem = mensagem;
        Complemento = complemento;
    }

    public string Entidade { get; }
    public LogNivel NivelLog { get; }
    public string Mensagem { get; }
    public string? Complemento { get; }
}