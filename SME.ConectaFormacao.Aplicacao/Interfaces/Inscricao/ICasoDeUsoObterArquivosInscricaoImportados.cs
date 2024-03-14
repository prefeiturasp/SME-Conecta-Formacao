using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao
{
    public interface ICasoDeUsoObterArquivosInscricaoImportados
    {
        Task<PaginacaoResultadoDTO<ArquivoInscricaoImportadoDto>> Executar(long propostaId);
    }
}
