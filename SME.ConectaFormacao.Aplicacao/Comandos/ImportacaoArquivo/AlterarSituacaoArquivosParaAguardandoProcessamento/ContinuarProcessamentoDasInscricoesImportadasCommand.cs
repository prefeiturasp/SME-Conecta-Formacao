using MediatR;

namespace SME.ConectaFormacao.Aplicacao.Comandos.ImportacaoArquivo.AlterarSituacaoArquivosParaAguardandoProcessamento
{
    public record ContinuarProcessamentoDasInscricoesImportadasCommand(long ArquivoImportacaoId) : IRequest<bool>;
}