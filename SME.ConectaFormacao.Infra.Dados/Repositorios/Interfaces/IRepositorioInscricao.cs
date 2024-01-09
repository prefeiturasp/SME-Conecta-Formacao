using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioInscricao : IRepositorioBaseAuditavel<Inscricao>
    {
        Task<bool> ConfirmarInscricaoVaga(Inscricao inscricao);
        Task<bool> ExisteInscricaoNaProposta(long propostaId, long usuarioId);
        Task<int> LiberarInscricaoVaga(Inscricao inscricao);
        Task<string> ObterCargoFuncaoPorId(long id);
        Task<IEnumerable<Inscricao>> ObterDadosPaginadosPorUsuarioId(long usuarioId, int numeroPagina, int numeroRegistros);
        Task<int> ObterTotalRegistrosPorUsuarioId(long usuarioId);
        Task<IEnumerable<Inscricao>> ObterInscricaoPorIdComFiltros(long inscricaoId,string? login,string? cpf,string? nomeCursista);

        Task<IEnumerable<PropostaTurma>> ObterDadosPaginadosComFiltros(long usuarioId, long? codigoDaFormacao,
            string? nomeFormacao, int numeroPagina, int numeroRegistros);
    }
}
