using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Log;

namespace SME.ConectaFormacao.Aplicacao.Comandos.SalvarLog;

public class SalvarLogCommand(LogDTO logDTO) : IRequest<bool>
{
    public LogDTO LogDTO { get; } = logDTO;
}