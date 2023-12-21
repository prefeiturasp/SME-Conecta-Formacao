using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

namespace SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces
{
    public interface IServicoEol
    {
        Task<string> ObterNomeProfissionalPorRegistroFuncional(string registroFuncional);
        Task<IEnumerable<DreNomeAbreviacaoDTO>> ObterCodigosDres();
        Task<IEnumerable<ComponenteCurricularAnoTurmaEOLDTO>> ObterComponentesCurricularesEAnosTurmaPorAnoLetivo(int anoLetivo);
        Task<IEnumerable<CargoFuncionarioConectaDTO>> ObterCargosFuncionadoPorRegistroFuncional(string registroFuncional);
    }
}