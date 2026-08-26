using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafDeclaracoes;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafDeclaracoes
{
    public interface ICasoDeUsoListarTodasDeclaracoesCodaf
    {
        Task<Resultado<PaginacaoResultadoDto<ListagemDeclaracoesCodafDto>>> ExecutarAsync(FiltroListagemTodasDeclaracoesCodafDto filtro);
    }
}
