using SME.ConectaFormacao.Infra.Dados.Dtos.Propostas;

namespace SME.ConectaFormacao.Infra.Dados.Relatorios
{
    public interface IGeradorLaudaDocxService
    {
        Task<byte[]> GerarArquivoLaudaCompletaAsync(PropostaLaudaCompletaDto dados);
    }
}
