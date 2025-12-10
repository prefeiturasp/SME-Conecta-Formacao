using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes
{
    public interface ICasoDeUsoObterArquivosInscricaoImportados
    {
        Task<PaginacaoResultadoDto<ArquivoInscricaoImportadoDTO>> Executar(long propostaId);
    }
}
