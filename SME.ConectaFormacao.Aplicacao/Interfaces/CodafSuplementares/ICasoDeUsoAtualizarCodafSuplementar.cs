using SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares
{
    public interface ICasoDeUsoAtualizarCodafSuplementar
    {
        Task<Resultado> ExecutarAsync(CodafSuplementarCadastroDto codafSuplementarCadastroDto, long id);
    }
}
