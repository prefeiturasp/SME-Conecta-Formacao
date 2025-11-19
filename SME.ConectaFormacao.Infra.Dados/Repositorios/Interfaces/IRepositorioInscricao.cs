using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioInscricao : IRepositorioBaseAuditavel<Inscricao>
    {
        Task<bool> ConfirmarInscricaoVaga(Inscricao inscricao);
        Task<bool> UsuarioEstaInscritoNaProposta(long propostaId, long usuarioId);
        Task<int> LiberarInscricaoVaga(Inscricao inscricao);
        Task<CargoFuncaoDTO> ObterCargoFuncaoPorId(long id);
        Task<IEnumerable<Inscricao>> ObterDadosPaginadosPorUsuarioId(long usuarioId, int numeroPagina, int numeroRegistros);
        Task<int> ObterTotalRegistrosPorUsuarioId(long usuarioId);
        Task<IEnumerable<Inscricao>> ObterInscricaoPorIdComFiltros(long inscricaoId, string? login, string? cpf, string? nomeCursista, long[]? turmasId, int numeroPagina, int numeroRegistros, bool ocultarCancelada = false,
    bool ocultarTransferida = false);
        Task<IEnumerable<Proposta>> ObterDadosPaginadosComFiltros(long? areaPromotoraIdUsuarioLogado, long? codigoDaFormacao, string? nomeFormacao, int numeroPagina, int numeroRegistros, long? numeroHomologacao);
        Task<IEnumerable<ListagemFormacaoComTurmaDTO>> DadosListagemFormacaoComTurma(long[] propostaIds, long? propostaTurmaId = null);
        Task<int> ObterDadosPaginadosComFiltrosTotalRegistros(long? areaPromotoraIdUsuarioLogado, long? codigoDaFormacao, string? nomeFormacao, long? numeroHomologacao);
        Task<int> ObterInscricaoPorIdComFiltrosTotalRegistros(long propostaId, string? login, string? cpf, string? nomeCursista, long[]? turmasId, bool ocultarCancelada = false,
    bool ocultarTransferida = false);
        Task<IEnumerable<PropostaTipoInscricao>> ObterTiposInscricaoPorPropostaIds(long[] codigosFormacao);
        Task<IEnumerable<Inscricao>> ObterInscricoesConfirmadas();
        Task<IEnumerable<InscricaoUsuarioInternoDto>> ObterInscricoesPorPropostasTurmasIdUsuariosInternos(long[] propostasTurmasId);
        Task<IEnumerable<InscricaoPossuiAnexoDTO>> ObterSeInscricaoPossuiAnexoPorPropostasIds(long[] inscricoesId);
        Task<IEnumerable<InscricaoDadosEmailConfirmacao>> ObterDadosInscricaoPorInscricaoId(long inscricoeId);
        Task<IEnumerable<long>> ObterIdsInscricoesAguardandoAnalise(long id);
        Task AtualizarSituacao(long inscricaoId, SituacaoInscricao situacao);
    }
}
