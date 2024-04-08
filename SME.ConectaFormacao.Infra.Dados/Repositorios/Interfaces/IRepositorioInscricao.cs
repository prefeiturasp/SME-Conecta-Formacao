using SME.ConectaFormacao.Dominio.Entidades;
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
        Task<IEnumerable<Inscricao>> ObterInscricaoPorIdComFiltros(long inscricaoId, string? login, string? cpf, string? nomeCursista, long[]? turmasId, int numeroPagina, int numeroRegistros);
        Task<IEnumerable<Proposta>> ObterDadosPaginadosComFiltros(long? areaPromotoraIdUsuarioLogado, long? codigoDaFormacao, string? nomeFormacao, int numeroPagina, int numeroRegistros);
        Task<IEnumerable<ListagemFormacaoComTurmaDTO>> DadosListagemFormacaoComTurma(long[] propostaIds);
        Task<int> ObterDadosPaginadosComFiltrosTotalRegistros(long? areaPromotoraIdUsuarioLogado, long? codigoDaFormacao, string? nomeFormacao);
        Task<IEnumerable<PropostaTipoInscricao>> ObterTiposInscricaoPorPropostaIds(long[] codigosFormacao);
        Task<IEnumerable<Inscricao>> ObterInscricoesConfirmadas();
        Task<int> ObterInscricaoPorIdComFiltrosTotalRegistros(long inscricaoId, string? login, string? cpf, string? nomeCursista, long[]? turmaId);
    }
}
