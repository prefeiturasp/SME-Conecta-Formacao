using SME.ConectaFormacao.Aplicacao.Dtos.CodafConferencias;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafConferencias
{
    public interface ICasoDeUsoAtualizarCodafConferencia
    {
        Task<Resultado> ExecutarAsync(CodafConferenciaCadastroDto dto, long id);
    }
}
