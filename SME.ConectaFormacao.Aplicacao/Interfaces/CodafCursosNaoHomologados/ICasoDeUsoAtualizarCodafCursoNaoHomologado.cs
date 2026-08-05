using SME.ConectaFormacao.Aplicacao.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafCursosNaoHomologados
{
    public interface ICasoDeUsoAtualizarCodafCursoNaoHomologado
    {
        Task<Resultado> ExecutarAsync(CodafCursoNaoHomologadoCadastroDto codafCursoNaoHomologadoCadastroDto, long id);
    }
}