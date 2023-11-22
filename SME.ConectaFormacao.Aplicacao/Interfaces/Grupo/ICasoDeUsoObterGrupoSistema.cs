using SME.ConectaFormacao.Aplicacao.Dtos.Grupo;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Grupo
{
    public interface ICasoDeUsoObterGrupoSistema
    {
        Task<IEnumerable<GrupoDTO>> Executar();
    }
}
