using SME.ConectaFormacao.Aplicacao.Dtos.Dre;

namespace SME.ConectaFormacao.Aplicacao
{
    public interface ICasoDeUsoObterListaDre
    {
        Task<IEnumerable<DreDTO>> Executar(bool exibirTodos);
    }
}