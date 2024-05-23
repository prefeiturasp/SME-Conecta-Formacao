namespace SME.Conecta.Notificacao.Worker
{
    public class WorkerRabbitMQNotificacaoConstantes
    {
        public const string Exchange = "sme.conecta.notificacao.workers";
        public const string ExchangeDeadLetter = "sme.conecta.notificacao.workers.deadletter";
        public const int DeadLetterTtl = 10 * 60 * 1000; /*10 Min * 60 Seg * 1000 milisegundos = 10 minutos em milisegundos*/

        public const int QuantidadeReprocessamentoDeadLetter = 3;


        public const string EnviarNotificacaoCriadaUsuarios = "conecta.notificacao.criada.usuarios";
        public const string EnviarNotificacaoCriadaGrupos = "conecta.notificacao.criada.grupos";

        public const string EnviarNotificacaoLidaUsuarios = "conecta.notificacao.lida.usuarios";
        public const string EnviarNotificacaoLidaGrupos = "conecta.notificacao.lida.grupos";

        public const string EnviarNotificacaoExcluidaUsuarios = "conecta.notificacao.excluida.usuarios";
        public const string EnviarNotificacaoExcluidaGrupos = "conecta.notificacao.excluida.grupos";
    }
}
