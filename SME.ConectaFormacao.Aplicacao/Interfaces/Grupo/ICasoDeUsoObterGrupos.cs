using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Grupo
{
    public interface ICasoDeUsoObterGrupos
    {
        Task<IEnumerable<GrupoDTO>> Executar();
    }
}
