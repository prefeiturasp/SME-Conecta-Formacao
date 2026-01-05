using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.Inscricoes;

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
        Task<ResultadoPaginado<Inscricao>> ObterInscricoesPorPropostaPaginadasAsync(FiltroListagemInscricaoDto filtro);
        Task<IEnumerable<Proposta>> ObterDadosPaginadosComFiltros(long? areaPromotoraIdUsuarioLogado, long? codigoDaFormacao, string? nomeFormacao, int numeroPagina, int numeroRegistros, long? numeroHomologacao);
        Task<IEnumerable<ListagemFormacaoComTurmaDTO>> DadosListagemFormacaoComTurma(long[] propostaIds, long? propostaTurmaId = null);
        Task<int> ObterDadosPaginadosComFiltrosTotalRegistros(long? areaPromotoraIdUsuarioLogado, long? codigoDaFormacao, string? nomeFormacao, long? numeroHomologacao);
        Task<IEnumerable<PropostaTipoInscricao>> ObterTiposInscricaoPorPropostaIds(long[] codigosFormacao);
        Task<IEnumerable<Inscricao>> ObterInscricoesConfirmadas();
        Task<IEnumerable<InscricaoUsuarioInternoDto>> ObterInscricoesPorPropostasTurmasIdUsuariosInternos(long[] propostasTurmasId);
        Task<IEnumerable<InscricaoPossuiAnexoDTO>> ObterSeInscricaoPossuiAnexoPorPropostasIds(long[] inscricoesId);
        Task<IEnumerable<InscricaoDadosEmailConfirmacao>> ObterDadosInscricaoPorInscricaoId(long inscricoeId);
        Task<IEnumerable<long>> ObterIdsInscricoesAguardandoAnalise(long id);
        Task AtualizarSituacao(long inscricaoId, SituacaoInscricao situacao);
    }
}
