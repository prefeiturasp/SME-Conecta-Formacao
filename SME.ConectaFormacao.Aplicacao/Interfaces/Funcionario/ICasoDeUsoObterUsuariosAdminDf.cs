using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Funcionario
{
    public interface ICasoDeUsoObterUsuariosAdminDf
    {
        Task<IEnumerable<RetornoUsuarioLoginNomeDTO>> Executar();
    }
}