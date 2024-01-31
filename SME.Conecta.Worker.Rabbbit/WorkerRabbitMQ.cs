using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Mensageria;
using SME.ConectaFormacao.Infra.Servicos.Telemetria;
using SME.ConectaFormacao.Infra.Servicos.Telemetria.Options;

namespace SME.Conecta.Worker
{
    public sealed class WorkerRabbitMQ : WorkerRabbitConecta
    {
        public WorkerRabbitMQ(IServiceScopeFactory serviceScopeFactory,
            IServicoTelemetria servicoTelemetria,
            IServicoMensageriaConecta servicoMensageria,
            IServicoMensageriaMetricas servicoMensageriaMetricas,
            IOptions<TelemetriaOptions> telemetriaOptions,
            IOptions<ConsumoFilasOptions> consumoFilasOptions,
            IConnectionFactory factory)
            : base(serviceScopeFactory, servicoTelemetria, servicoMensageria, servicoMensageriaMetricas, telemetriaOptions, consumoFilasOptions, factory, "WorkerRabbitMQ", typeof(RotasRabbit))
        {
            RegistrarUseCases();
        }

        protected override void RegistrarUseCases()
        {
            Comandos.Add(RotasRabbit.SincronizaEstruturaInstitucionalDre, new ComandoRabbit("Estrutura Institucional - Obter Dre", typeof(IExecutarSincronizacaoInstitucionalDreSyncUseCase), true));
            Comandos.Add(RotasRabbit.SincronizaEstruturaInstitucionalDreTratar, new ComandoRabbit("Estrutura Institucional - Tratar uma Dre", typeof(IExecutarSincronizacaoInstitucionalDreTratarUseCase), true));

            Comandos.Add(RotasRabbit.SincronizaComponentesCurricularesEAnosTurmaEOL, new ComandoRabbit("Sincronização de Componentes Curriculares e Anos da Turma do EOL", typeof(IExecutarSincronizacaoComponentesCurricularesEAnosTurmaEOLUseCase), true));

            Comandos.Add(RotasRabbit.GerarPropostaTurmaVaga, new ComandoRabbit("Gerar Tabela Proposta Turma Vaga", typeof(ICasoDeUsoGerarPropostaTurmaVaga), true));

            Comandos.Add(RotasRabbit.RealizarInscricaoAutomatica, new ComandoRabbit("Realizar Inscrições Automáticas - Obter Formações e Cursistas", typeof(ICasoDeUsoRealizarInscricaoAutomatica), true));
            Comandos.Add(RotasRabbit.RealizarInscricaoAutomaticaTratarTurmas, new ComandoRabbit("Realizar Inscrições Automáticas - Tratar as turmas - incluir novas conforme a quantidade de cursistas", typeof(ICasoDeUsoRealizarInscricaoAutomaticaTratarTurmas), true));
            Comandos.Add(RotasRabbit.RealizarInscricaoAutomaticaTratarCursistas, new ComandoRabbit("Realizar Inscrições Automáticas - Iterar sobre os cursistas e enviar para fila de inscrição", typeof(ICasoDeUsoRealizarInscricaoAutomaticaTratarCursista), true));
            Comandos.Add(RotasRabbit.RealizarInscricaoAutomaticaIncreverCursista, new ComandoRabbit("Realizar Inscrições Automáticas - Coletar usuário e inscrever cursista", typeof(ICasoDeUsoRealizarInscricaoAutomaticaInscreverCursista), true));
        }
    }
}