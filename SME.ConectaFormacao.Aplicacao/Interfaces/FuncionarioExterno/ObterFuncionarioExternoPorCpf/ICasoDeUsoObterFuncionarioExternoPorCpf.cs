using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.FuncionarioExterno.ObterFuncionarioExternoPorCpf
{
    public interface ICasoDeUsoObterFuncionarioExternoPorCpf
    {
        Task<IEnumerable<FuncionarioExternoServicoEol>> Executar(string cpf);
    }
}