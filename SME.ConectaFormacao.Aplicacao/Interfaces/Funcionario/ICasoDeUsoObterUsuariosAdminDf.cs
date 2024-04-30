using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Funcionario;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Funcionario
{
    public interface ICasoDeUsoObterUsuariosAdminDf
    {
        Task<IEnumerable<RetornoUsuarioLoginNomeDTO>> Executar();
    }
}