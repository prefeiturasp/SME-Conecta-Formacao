namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao
{
    public interface ICasoDeUsoAlterarSituacaoArquivosParaAguardandoProcessamento
    {
        Task<bool> Executar(long propostaId);
    }
}
