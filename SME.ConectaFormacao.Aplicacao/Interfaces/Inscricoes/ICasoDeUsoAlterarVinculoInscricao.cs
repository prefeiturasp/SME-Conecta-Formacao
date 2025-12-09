using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes
{
    public interface ICasoDeUsoAlterarVinculoInscricao
    {
        Task<bool> Executar(long id, AlterarCargoFuncaoVinculoIncricaoDTO alterarCargoFuncaoVinculoIncricao);
    }
}