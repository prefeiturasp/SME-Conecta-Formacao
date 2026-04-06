using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf.Dependencias
{
    public record CodafListaPresencaDependencias(
        IRepositorioCodafListaPresenca RepositorioLista,
        IRepositorioCodafRetificacaoListaPresenca RepositorioRetificacao,
        ICodafInscritosListaPresencaService InscritosService,
        IValidadorCodafListaPresencaService ValidadorDominio,
        IGerenciadorAnexosCodafService AnexosService,
        IGerenciadorMovimentacaoCodafService MovimentacaoService);
}