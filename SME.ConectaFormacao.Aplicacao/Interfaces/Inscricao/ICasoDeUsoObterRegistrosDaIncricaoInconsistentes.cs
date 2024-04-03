using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao
{
    public interface ICasoDeUsoObterRegistrosDaIncricaoInconsistentes
    {
        Task<PaginacaoResultadoDTO<RegistroDaInscricaoInsconsistenteDTO>> Executar(long arquivoId);
    }
}
