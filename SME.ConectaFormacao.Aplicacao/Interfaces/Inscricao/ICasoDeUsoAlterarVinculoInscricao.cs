using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao
{
    public interface ICasoDeUsoAlterarVinculoInscricao
    {
        Task<bool> Executar(long id, AlterarCargoFuncaoVinculoIncricaoDTO alterarCargoFuncaoVinculoIncricao);
    }
}