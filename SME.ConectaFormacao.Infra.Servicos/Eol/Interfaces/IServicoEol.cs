namespace SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces
{
    public interface IServicoEol
    {
        Task<CursistaResumidoServicoEol> ObterNomeCpfProfissionalPorRegistroFuncional(string registroFuncional);
        Task<IEnumerable<DreServicoEol>> ObterCodigosDres();
        Task<IEnumerable<ComponenteCurricularAnoTurmaServicoEol>> ObterComponentesCurricularesEAnosTurmaPorAnoLetivo(int anoLetivo);
        Task<IEnumerable<CursistaCargoServicoEol>> ObterCargosFuncionadoPorRegistroFuncional(string registroFuncional);
        Task<IEnumerable<CursistaServicoEol>> ObterFuncionariosPorCargosFuncoesModalidadeAnosComponentesDres(IEnumerable<long> codigosCargos,
            IEnumerable<long> codigosFuncoes, IEnumerable<long> codigosModalidades, IEnumerable<string> anosTurma,
            IEnumerable<string> codigosDres, IEnumerable<long> codigosComponentesCurriculares, bool ehTipoJornadaJEIF);

        Task<UnidadeEol> ObterUnidadePorCodigoEol(string codigoEol);
        Task<IEnumerable<FuncionarioExternoServicoEol>?> ObterDadosFuncionarioExternoPorCpf(string cpf);
        Task<IEnumerable<DreUeAtribuicaoServicoEol>> ObterDreUeAtribuicaoPorFuncionarioCargo(string registroFuncional, long codigoCargo);
        Task<IEnumerable<UsuarioPerfilServicoEol>> ObterUsuariosPorPerfis(IEnumerable<Guid> perfis);
        Task<bool> VerificarSeUsuarioEstaAtivo(string rf);
    }
}