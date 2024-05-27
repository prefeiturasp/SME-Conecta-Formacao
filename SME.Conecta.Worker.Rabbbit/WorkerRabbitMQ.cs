using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Email;
using SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo;
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

            Comandos.Add(RotasRabbit.RealizarImportacaoInscricaoCursistaValidar, new ComandoRabbit("Realizar a busca dos itens da importação das inscrições de cursistas de forma paginada para processar a validação dos itens", typeof(ICasoDeUsoImportacaoInscricaoCursistaValidar), true));
            Comandos.Add(RotasRabbit.RealizarImportacaoInscricaoCursistaValidarItem, new ComandoRabbit("Realiza a validação de cada linha da importação da inscrição do cursista", typeof(ICasoDeUsoImportacaoInscricaoCursistaValidarItem), true));

            Comandos.Add(RotasRabbit.ProcessarArquivoDeImportacaoInscricao, new ComandoRabbit("Processar arquivo de importação e seus registro validos", typeof(ICasoDeUsoProcessarArquivoDeImportacaoInscricao), true));
            Comandos.Add(RotasRabbit.ProcessarRegistroDoArquivoDeImportacaoInscricao, new ComandoRabbit("Processar registro do arquivo de importação", typeof(ICasoDeUsoProcessarRegistroDoArquivoDeImportacaoInscricao), true));


            Comandos.Add(RotasRabbit.AtualizarCargoFuncaoVinculoInscricaoCursista, new ComandoRabbit("Atualizar o cargo, função e vínculo da inscrição do cursista.", typeof(ICasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursista), true));
            Comandos.Add(RotasRabbit.AtualizarCargoFuncaoVinculoInscricaoCursistaTratar, new ComandoRabbit("Atualizar o cargo, função e vínculo da inscrição do cursista - Tratar por cursista.", typeof(ICasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursistaTratar), true));

            Comandos.Add(RotasRabbit.EnviarEmailDevolverProposta, new ComandoRabbit("Enviar e-mail ao devolver uma proposta", typeof(ICasoDeUsoEnviarEmailDevolverProposta), true));

            Comandos.Add(RotasRabbit.EncerrarInscricaoAutomaticamente, new ComandoRabbit("Encerrar Inscrição Quando o servidor ficar inativo no EOL", typeof(ICasoDeUsoEncerrarInscricaoCursistaInativoSemCargo), true));
            Comandos.Add(RotasRabbit.EncerrarInscricaoAutomaticamenteTurma, new ComandoRabbit("Encerrar Inscrição Quando o servidor ficar inativo no EOL - Turma", typeof(ICasoDeUsoEncerrarInscricaoAutomaticamenteTurma), true));
            Comandos.Add(RotasRabbit.EncerrarInscricaoAutomaticamenteInscricoes, new ComandoRabbit("Encerrar Inscrição Quando o servidor ficar inativo no EOL - Inscrições", typeof(ICasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes), true));
            Comandos.Add(RotasRabbit.EncerrarInscricaoAutomaticamenteUsuarios, new ComandoRabbit("Encerrar Inscrição Quando o servidor ficar inativo no EOL -  Usuarios", typeof(ICasoDeUsoEncerrarInscricaoAutomaticamenteUsuarios), true));
            
            Comandos.Add(RotasRabbit.NotificarPareceristasSobreAtribuicaoPelaDF, new ComandoRabbit("Notificar pareceristas sobre atribuicao pela DF", typeof(ICasoDeUsoNotificarPareceristasSobreAtribuicaoPelaDF), true));
            Comandos.Add(RotasRabbit.NotificarDFPeloEnvioParecerPeloParecerista, new ComandoRabbit("Notificar DF pelo envio parecer pelo parecerista", typeof(ICasoDeUsoNotificarDFPeloEnvioParecerPeloParecerista), true));
            Comandos.Add(RotasRabbit.NotificarAreaPromotoraParaAnaliseParecer, new ComandoRabbit("Notificar área promotora para análise do parecer", typeof(ICasoDeUsoNotificarAreaPromotoraParaAnaliseParecer), true));
            Comandos.Add(RotasRabbit.NotificarPareceristasParaReanalise, new ComandoRabbit("Notificar parecerista para reanálise", typeof(ICasoDeUsoNotificarPareceristasParaReanalise), true));
            Comandos.Add(RotasRabbit.NotificarResponsavelDFSobreReanaliseDoParecerista, new ComandoRabbit("Notificar responsável DF sobre reanálise do parecerista", typeof(ICasoDeUsoNotificarResponsavelDFSobreReanaliseDoParecerista), true));
            Comandos.Add(RotasRabbit.NotificarAreaPromotoraSobreValidacaoFinalPelaDF, new ComandoRabbit("Notificar Área promotora sobre a validação final pela DF", typeof(ICasoDeUsoNotificarResponsavelDFSobreReanaliseDoParecerista), true));
            
            Comandos.Add(RotasRabbit.EnviarEmail, new ComandoRabbit("Enviar e-mail", typeof(ICasoDeUsoEnviarEmail), true));
            Comandos.Add(RotasRabbit.EnviarNotificacao, new ComandoRabbit("Enviar notificação via SignalR", typeof(ICasoDeUsoEnviarNotificacao), true));
        }
    }
}