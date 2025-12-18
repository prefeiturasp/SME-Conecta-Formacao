using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Codaf
{
    public interface ICasoDeUsoCriarCodafListaPresenca
    {
        Task<Resultado<CodafListaPresencaDto>> ExecutarAsync(CodafListaPresencaCadastroDto codafListaPresencaCadastroDto);
    }
}