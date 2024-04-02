using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao
{
    public interface ICasoDeUsoObterRegistrosDaIncricaoInconsistentes
    {
        Task<PaginacaoResultadoComMensagemDTO<RegistroDaInscricaoInsconsistenteDTO>> Executar(long arquivoId);
    }
}
