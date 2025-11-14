using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Usuario
{
    public interface ICasoDeUsoObterUsuariosPorEolUnidade
    {
        Task<IEnumerable<DadosLoginUsuarioDto>> ExecutarAsync(string codigoEolUnidade, string? login, string? nome);
    }
}