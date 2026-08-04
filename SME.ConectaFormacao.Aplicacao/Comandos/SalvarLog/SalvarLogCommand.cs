using MediatR;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Comandos.SalvarLog;

public class SalvarLogCommand(
    string entidade, LogNivel nivelLog, string mensagem, string? complemento = null, 
    Guid? identificadorRastreamento = null, Exception? excecao = null) : IRequest<bool>
{
    public string Entidade { get; } = entidade;
    public LogNivel NivelLog { get; } = nivelLog;
    public string Mensagem { get; } = mensagem;
    public string? Complemento { get; } = complemento;
    public Guid IdentificadorRastreamento { get; } = identificadorRastreamento ?? Guid.NewGuid();
    public Exception? Excecao { get; } = excecao;
}