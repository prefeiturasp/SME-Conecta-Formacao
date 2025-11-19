using MediatR;
using SME.ConectaFormacao.Aplicacao.CasosDeUso;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao;

public class ExecutarSincronizacaoInstitucionalDreTratarUseCase : CasoDeUsoAbstrato, IExecutarSincronizacaoInstitucionalDreTratarUseCase
{
    public ExecutarSincronizacaoInstitucionalDreTratarUseCase(IMediator mediator) : base(mediator)
    {
    }

    public async Task<bool> Executar(MensagemRabbit param)
    {
        var dre = param.ObterObjetoMensagem<DreServicoEol>();

        await mediator.Send(new TrataSincronizacaoInstitucionalDreCommand(dre));

        return true;
    }
}