using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

namespace SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces
{
    public interface IServicoEol
    {
        Task<string> ObterNomeProfissionalPorRegistroFuncional(string registroFuncional);
        Task<IEnumerable<DreNomeAbreviacaoDTO>> ObterCodigosDres();
        Task<IEnumerable<ComponenteCurricularEOLDTO>> ObterComponentesCurricularesEAnoPorAnoLetivo(int anoLetivo);
    }
}