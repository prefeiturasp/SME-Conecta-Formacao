namespace SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces
{
    public interface IServicoEol
    {
        Task<string> ObterNomeProfissionalPorRegistroFuncional(string registroFuncional);
        Task<IEnumerable<DreServicoEol>> ObterCodigosDres();
        Task<IEnumerable<ComponenteCurricularAnoTurmaServicoEol>> ObterComponentesCurricularesEAnosTurmaPorAnoLetivo(int anoLetivo);
        Task<IEnumerable<CursistaCargoServicoEol>> ObterCargosFuncionadoPorRegistroFuncional(string registroFuncional);
        Task<IEnumerable<CursistaServicoEol>> ObterFuncionariosPorCargosFuncoesModalidadeAnosComponentesDres(IEnumerable<long> codigosCargos, 
            IEnumerable<long> codigosFuncoes, IEnumerable<long> codigosModalidades, IEnumerable<string> anosTurma, 
            IEnumerable<string> codigosDres, IEnumerable<long> codigosComponentesCurriculares, bool ehTipoJornadaJEIF);
    }
}