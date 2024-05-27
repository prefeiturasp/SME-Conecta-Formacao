namespace SME.ConectaFormacao.Infra
{
    public static class RotasRabbitNotificacao
    {
        public const string Exchange = "sme.conecta.notificacao.workers";

        public const string EnviarNotificacaoCriadaUsuarios = "conecta.notificacao.criada.usuarios";
        public const string EnviarNotificacaoLidaUsuarios = "conecta.notificacao.lida.usuarios";
    }
}