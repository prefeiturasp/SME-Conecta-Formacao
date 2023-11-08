using MediatR;
using SME.ConectaFormacao.Aplicacao.CasosDeUso;
using SME.ConectaFormacao.Aplicacao.Comandos.SalvarLogViaRabbit;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ExecutarSincronizacaoInstitucionalDreSyncUseCase : CasoDeUsoAbstrato, IExecutarSincronizacaoInstitucionalDreSyncUseCase
    {
        public ExecutarSincronizacaoInstitucionalDreSyncUseCase(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var dres = await mediator.Send(ObterCodigosDresQuery.Instance);
            if (dres.EhNulo() || !dres.Any())
            {
                throw new NegocioException("Não foi possível localizar as Dres no EOL para a sincronização instituicional");
            }

            foreach (var dre in dres)
            {
                try
                {
                    var publicarTratamentoDre = await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.SincronizaEstruturaInstitucionalDreTratar, dre, param.CodigoCorrelacao, null));
                    if (!publicarTratamentoDre)
                    {
                        await mediator.Send(new SalvarLogViaRabbitCommand($"Não foi possível inserir a Dre : {publicarTratamentoDre} na fila de sync.", LogNivel.Negocio, LogContexto.SincronizacaoInstitucional));
                    }
                }
                catch (Exception)
                {
                    throw new NegocioException($"Não foi possível realizar a sincronização da dre ${dre?.Nome}");
                }
            }

            return true;
        }
    }
}