using System.Threading.Tasks;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Relatorios
{
    public interface ICasoDeUsoObterRelatorioLaudaCompletaDocx
    {
        Task<byte[]> ExecutarAsync(long propostaId);
    }
}
