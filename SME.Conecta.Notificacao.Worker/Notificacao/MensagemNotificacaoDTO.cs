namespace SME.Conecta.Notificacao.Worker.Interfaces
{
    public abstract class MensagemNotificacaoDTO
    {
        public long Id { get; set; }
        public DateTime DataHora { get; set; }
        public string Titulo { get; set; }
    }
}
