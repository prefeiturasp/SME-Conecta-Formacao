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
            var codigosDre = await mediator.Send(ObterCodigosDresQuery.Instance);
            if (codigosDre.EhNulo() || !codigosDre.Any())
            {
                throw new NegocioException("Não foi possível localizar as Dres no EOL para a sincronização instituicional");
            }

            foreach (var codigoDre in codigosDre)
            {
                try
                {
                    var publicarTratamentoDre = await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.SincronizaEstruturaInstitucionalDreTratar, codigoDre, param.CodigoCorrelacao, null));
                    if (!publicarTratamentoDre)
                    {
                        await mediator.Send(new SalvarLogViaRabbitCommand($"Não foi possível inserir a Dre : {publicarTratamentoDre} na fila de sync.", LogNivel.Negocio, LogContexto.SincronizacaoInstitucional));
                    }
                }
                catch (Exception)
                {
                    throw new NegocioException($"Não foi possível realizar a sincronização da dre ${codigosDre}");
                }
            }

            return true;
        }
    }
}