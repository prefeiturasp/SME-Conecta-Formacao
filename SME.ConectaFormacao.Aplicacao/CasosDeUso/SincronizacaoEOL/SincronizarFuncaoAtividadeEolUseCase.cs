using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Comandos.SalvarLogViaRabbit;
using SME.ConectaFormacao.Aplicacao.Interfaces.SincronizacaoEOL;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.SincronizacaoEOL
{
    public class SincronizarFuncaoAtividadeEolUseCase(IMediator mediator)
        : CasoDeUsoAbstrato(mediator), ISincronizarFuncaoAtividadeEolUseCase
    {
        public async Task<bool> Executar(MensagemRabbit param)
        {
            var codigosDre = new List<string> { "SME" };
            var dresEol = await mediator.Send(new ObterCodigosDresEOLQuery());
            if (dresEol?.Any() == true)
                codigosDre.AddRange(dresEol.Select(d => d.Codigo));

            Task LogNegocioAsync(string dre) =>
                mediator.Send(new SalvarLogViaRabbitCommand(
                    $"Erro ao publicar mensagem na fila para sincronização de cargos EOL da DRE {dre}.",
                    LogNivel.Negocio,
                    LogContexto.SincronizacaoCargosEol));

            Task LogErroCriticoAsync(string dre, Exception ex) =>
                mediator.Send(new SalvarLogViaRabbitCommand(
                    $"Erro ao sincronizar cargos EOL da DRE {dre}.",
                    LogNivel.Critico,
                    LogContexto.SincronizacaoCargosEol,
                    "",
                    projeto: "ConectaFormacao",
                    rastreamento: ex.StackTrace ?? "",
                    excecaoInterna: ex.InnerException?.Message ?? "",
                    innerException: ex.InnerException?.StackTrace ?? ""));

            foreach (var dre in codigosDre)
            {
                try
                {
                    var publicado = await mediator.Send(
                        new PublicarNaFilaRabbitCommand(
                            RotasRabbit.SincronizaFuncaoAtividadeDre,
                            dre));

                    if (!publicado)
                        await LogNegocioAsync(dre);
                }
                catch (Exception ex)
                {
                    await LogErroCriticoAsync(dre, ex);
                }
            }

            return true;
        }
    }
}
