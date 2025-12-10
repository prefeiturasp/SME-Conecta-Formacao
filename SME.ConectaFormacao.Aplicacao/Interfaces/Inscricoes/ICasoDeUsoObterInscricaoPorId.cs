using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Infra.Dados.Dtos.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes
{
    public interface ICasoDeUsoObterInscricaoPorId
    {
        Task<PaginacaoResultadoDto<DadosListagemInscricaoDto>> ExecutarAsync(FiltroListagemInscricaoDto filtro);
    }
}