using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao
{
    public interface ICasoDeUsoObterDadosPaginadosComFiltros
    {
        Task<IEnumerable<DadosListagemFormacaoComTurma>> Executar(long? codigoDaFormacao, string? nomeFormacao);
    }
}