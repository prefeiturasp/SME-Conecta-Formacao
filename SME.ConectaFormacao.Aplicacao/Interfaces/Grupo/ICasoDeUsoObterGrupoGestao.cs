using SME.ConectaFormacao.Aplicacao.Dtos.Grupo;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Grupo
{
    public interface ICasoDeUsoObterGrupoGestao
    {
        Task<IEnumerable<GrupoGestaoDTO>> Executar();
    }
}
