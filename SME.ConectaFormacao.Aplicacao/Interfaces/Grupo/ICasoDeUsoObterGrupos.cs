using SME.ConectaFormacao.Aplicacao.Dtos.Grupo;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Grupo
{
    public interface ICasoDeUsoObterGrupos
    {
        Task<IEnumerable<GrupoDTO>> Executar();
    }
}
