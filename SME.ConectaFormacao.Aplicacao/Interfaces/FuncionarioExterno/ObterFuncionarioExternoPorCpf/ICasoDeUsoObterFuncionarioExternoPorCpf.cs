using SME.ConectaFormacao.Aplicacao.Dtos.FuncionarioExterno;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.FuncionarioExterno.ObterFuncionarioExternoPorCpf
{
    public interface ICasoDeUsoObterFuncionarioExternoPorCpf
    {
        Task<FuncionarioExternoDTO?> Executar(string cpf);
    }
}