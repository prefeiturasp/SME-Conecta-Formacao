namespace SME.ConectaFormacao.Infra.Servicos.Utilitarios;

public class ThreadPoolOptions
{
    public const string Secao = "ThreadPool";

    public int WorkerThreads { get; set; }
    public int CompletionPortThreads { get; set; }
}