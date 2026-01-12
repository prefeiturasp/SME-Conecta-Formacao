using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Dominio.Servicos.Interfaces
{
    public interface IGerenciadorMovimentacaoCodafService
    {
        Task RegistrarMovimentacaoAsync(CodafListaPresenca codaf, long? comentarioId = null);
    }
}
