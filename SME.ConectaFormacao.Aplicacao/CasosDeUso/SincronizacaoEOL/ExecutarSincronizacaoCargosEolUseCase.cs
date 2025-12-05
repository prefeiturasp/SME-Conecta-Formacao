using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Comandos.SalvarLogViaRabbit;
using SME.ConectaFormacao.Aplicacao.Interfaces.SincronizacaoEOL;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.SincronizacaoEOL
{
    public class ExecutarSincronizacaoCargosEolUseCase(IMediator mediator) : CasoDeUsoAbstrato(mediator), IExecutarSincronizacaoCargosEolUseCase
    {
        public async Task<bool> Executar(MensagemRabbit param)
        {
            List<string> codigosDre = ["SME"];
            var dres = await mediator.Send(new ObterCodigosDresEOLQuery());

            if (dres is not null && dres.Any())
                codigosDre.AddRange(dres.Select(d => d.Codigo));

            foreach (var codigoDre in codigosDre)
            {
                try
                {
                    var mensagemPublicada = await mediator.Send(new PublicarNaFilaRabbitCommand(
                        RotasRabbit.SincronizaCargosEolPorDre,
                        codigoDre));

                    if (!mensagemPublicada)
                        await mediator.Send(new SalvarLogViaRabbitCommand(
                            $"Erro ao publicar mensagem na fila para sincronização de cargos EOL da DRE {codigoDre}.",
                            LogNivel.Negocio,
                            LogContexto.SincronizacaoCargosEol));
                }
                catch (Exception ex)
                {
                    await mediator.Send(new SalvarLogViaRabbitCommand(
                        $"Erro ao sincronizar cargos EOL da DRE {codigoDre}.",
                        LogNivel.Critico,
                        LogContexto.SincronizacaoCargosEol,
                        "",
                        projeto: "ConectaFormacao",
                        rastreamento: ex.StackTrace ?? "",
                        excecaoInterna: ex.InnerException?.Message ?? "",
                        innerException: ex.InnerException?.StackTrace ?? ""));
                }
            }

            return true;
        }
    }
}
