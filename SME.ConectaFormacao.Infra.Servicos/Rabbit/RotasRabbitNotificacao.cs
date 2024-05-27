namespace SME.ConectaFormacao.Infra
{
    public static class RotasRabbitNotificacao
    {
        public const string EnviarNotificacaoCriadaUsuarios = "conecta.notificacao.criada.usuarios";
        public const string Exchange = "sme.conecta.notificacao.workers";
    }
}