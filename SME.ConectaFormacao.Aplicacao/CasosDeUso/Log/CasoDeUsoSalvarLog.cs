using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.SalvarLog;
using SME.ConectaFormacao.Aplicacao.Dtos.Log;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Log
{
    public class CasoDeUsoSalvarLog : CasoDeUsoAbstrato, ICasoDeUsoSalvarLog
    {
        public CasoDeUsoSalvarLog(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(LogDto logDto)
        {
            return await mediator.Send(new SalvarLogCommand(logDto.Entidade, logDto.NivelLog, logDto.Mensagem, logDto.Complemento));
        }
    }
}