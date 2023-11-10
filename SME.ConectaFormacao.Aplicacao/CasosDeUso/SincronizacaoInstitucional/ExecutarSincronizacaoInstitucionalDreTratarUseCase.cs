using MediatR;
using SME.ConectaFormacao.Aplicacao.CasosDeUso;
using SME.ConectaFormacao.Aplicacao.Comandos;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

namespace SME.ConectaFormacao.Aplicacao;

public class ExecutarSincronizacaoInstitucionalDreTratarUseCase : CasoDeUsoAbstrato, IExecutarSincronizacaoInstitucionalDreTratarUseCase
{
    public ExecutarSincronizacaoInstitucionalDreTratarUseCase(IMediator mediator) : base(mediator)
    {
    }

    public async Task<bool> Executar(MensagemRabbit param)
    {
        var dre = param.ObterObjetoMensagem<DreNomeAbreviacaoDTO>();

        await mediator.Send(new TrataSincronizacaoInstitucionalDreCommand(dre));

        return true;
    }
}