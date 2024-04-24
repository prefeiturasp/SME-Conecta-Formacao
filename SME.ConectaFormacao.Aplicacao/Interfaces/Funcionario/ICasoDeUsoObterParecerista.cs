using SME.ConectaFormacao.Aplicacao.Dtos.Funcionario;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Funcionario
{
    public interface ICasoDeUsoObterParecerista
    {
        Task<IEnumerable<UsuarioPareceristaDto>> Executar();
    }
}