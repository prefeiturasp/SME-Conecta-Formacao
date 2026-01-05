using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes
{
    public interface ICasoDeUsoObterRegistrosDaIncricaoInconsistentes
    {
        Task<PaginacaoResultadoComSucessoDTO<RegistroDaInscricaoInsconsistenteDto>> Executar(long arquivoId);
    }
}
