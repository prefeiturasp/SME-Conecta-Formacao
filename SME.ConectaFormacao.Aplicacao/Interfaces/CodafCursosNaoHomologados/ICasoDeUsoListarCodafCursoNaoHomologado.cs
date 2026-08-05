using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafCursosNaoHomologados
{
    public interface ICasoDeUsoListarCodafCursoNaoHomologado
    {
        Task<Resultado<PaginacaoResultadoDto<CodafCursoNaoHomologadoResumoDto>>> ExecutarAsync(FiltroCodafCursoNaoHomologadoDto filtro);
    }
}