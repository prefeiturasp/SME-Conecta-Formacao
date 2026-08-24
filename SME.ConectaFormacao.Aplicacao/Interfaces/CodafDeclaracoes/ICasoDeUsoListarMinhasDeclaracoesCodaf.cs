using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafDeclaracoes;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafDeclaracoes
{
    public interface ICasoDeUsoListarMinhasDeclaracoesCodaf
    {
        Task<Resultado<PaginacaoResultadoDto<MinhasDeclaracoesCodafDto>>> ExecutarAsync(FiltroListaMinhasDeclaracoesCodafDto filtro);
    }
}
