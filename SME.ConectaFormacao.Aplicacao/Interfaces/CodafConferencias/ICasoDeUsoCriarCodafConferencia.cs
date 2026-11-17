using SME.ConectaFormacao.Aplicacao.Dtos.CodafConferencias;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafConferencias
{
    public interface ICasoDeUsoCriarCodafConferencia
    {
        Task<Resultado<CodafConferenciaDetalhadoDto>> ExecutarAsync(CodafConferenciaCadastroDto dto);
    }
}
