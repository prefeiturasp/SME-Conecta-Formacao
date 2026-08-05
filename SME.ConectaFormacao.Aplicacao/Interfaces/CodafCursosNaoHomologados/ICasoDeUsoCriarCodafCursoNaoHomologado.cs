using SME.ConectaFormacao.Aplicacao.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafCursosNaoHomologados
{
    public interface ICasoDeUsoCriarCodafCursoNaoHomologado
    {
        Task<Resultado<CodafCursoNaoHomologadoDetalhadoDto>> ExecutarAsync(CodafCursoNaoHomologadoCadastroDto codafCursoNaoHomologadoCadastroDto);
    }
}