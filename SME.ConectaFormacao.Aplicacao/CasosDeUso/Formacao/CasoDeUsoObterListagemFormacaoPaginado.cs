using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Formacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Utilitarios;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Formacao
{
    public class CasoDeUsoObterListagemFormacaoPaginada(
        IMediator mediator, IContextoAplicacao contextoAplicacao, IUtilitariosPerfis utilitarios, ICacheDistribuido cacheDistribuido, IRepositorioProposta repositorioProposta) :
        CasoDeUsoAbstratoPaginado(mediator, contextoAplicacao), ICasoDeUsoObterListagemFormacaoPaginada
    {

        public async Task<PaginacaoResultadoDto<RetornoListagemFormacaoDTO>> Executar(FiltroListagemFormacaoDTO filtroListagemFormacaoDTO)
        {
            var chaveRedisFiltro = CacheDistribuidoNomes.FormacaoFiltro.Parametros(CacheFiltroFormacaoNomes.CHAVE_FILTRO_LISTAGEM_FORMACAO, contextoAplicacao.LoginUsuario);

            var propostasPaginadas = await repositorioProposta.ObterListagemFormacoesPorFiltro(new()
            {
                AreasPromotorasIds = filtroListagemFormacaoDTO.AreasPromotorasIds,
                DataFinal = filtroListagemFormacaoDTO.DataFinal,
                DataInicial = filtroListagemFormacaoDTO.DataInicial,
                FiltrarPorPerfil = utilitarios.FiltrarPorPerfil(),
                FormatosIds = filtroListagemFormacaoDTO.FormatosIds,
                Pagina = NumeroPagina,
                PalavrasChavesIds = filtroListagemFormacaoDTO.PalavrasChavesIds,
                TamanhoPagina = NumeroRegistros,
                PublicosAlvosIds = filtroListagemFormacaoDTO.PublicosAlvosIds,
                RfServidor = contextoAplicacao.LoginUsuario,
                Titulo = filtroListagemFormacaoDTO.Titulo
            });

            var formacoes = Enumerable.Empty<RetornoListagemFormacaoDTO>();
            if (propostasPaginadas.Itens.Any())
                formacoes = await mediator.Send(new ObterPropostasPorIdsQuery(propostasPaginadas.Itens));

            // Salvar o filtro utilizado no cache para reutilizar em outras páginas
            if (filtroListagemFormacaoDTO != null)
            {
                await cacheDistribuido.SalvarAsync(chaveRedisFiltro, filtroListagemFormacaoDTO);
            }

            return new PaginacaoResultadoDto<RetornoListagemFormacaoDTO>(formacoes, propostasPaginadas.TotalRegistros, NumeroRegistros);
        }
    }
}
