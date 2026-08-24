using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafDeclaracoes;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioCodafDeclaracao : IRepositorioBaseAuditavel<CodafDeclaracao>
    {
        Task<IEnumerable<DadosEmissaoDeclaracaoCodafDto>> ObterDadosParaEmissaoDeclaracoesCodafAsync(long codafNaoHomologadoId);
        Task InserirLoteAsync(IEnumerable<CodafDeclaracao> declaracoes);
        Task AtualizarStatusProcessamentoAsync(long id, StatusProcessamentoDeclaracaoCodaf statusProcessamento, string? chaveObjetoArmazenamento, string? erroProcessamento);
        Task<IList<CodafDeclaracao>> ObterDeclaracoesDisponiveisPorListaDeIdAsync(List<long> declaracoesId);
        Task AtualizaCodigoDeclaracao(long codafNaoHomologadoId);
        Task InativarDeclaracoesAnterioresCursistaAsync(IEnumerable<long> idInscritos);
        Task<IEnumerable<DadosProcessamentoCodafDto>> ObterDeclaracoesParaProcessamentoAsync();
        Task<ResultadoPaginado<MinhasDeclaracoesCodafDto>> ObterMinhasDeclaracoesPorFiltroAsync(FiltroMinhasDeclaracoesCodafDto filtro);
        Task<DadosDeclaracaoUsuarioParaDownloadDto?> ObterDeclaracaoDisponivelDoUsuarioAsync(long codafDeclaracaoId);
        Task<ResultadoPaginado<ListagemDeclaracoesCodafDto>> ObterTodasDeclaracoesAsync(FiltroListagemTodasDeclaracoesCodafDto filtro);
    }
}
