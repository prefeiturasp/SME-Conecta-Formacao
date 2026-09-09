using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;
using System.Globalization;
using System.Net;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterFormacaoDetalhadaPorIdQueryHandler(
        IRepositorioProposta repositorioProposta, IMapper mapper,
        IContextoAplicacao contextoAplicacao,
        IMediator mediator, ICacheDistribuido cacheDistribuido, IRepositorioUsuarioAcessibilidade repositorioUsuarioAcessibilidade) : 
        IRequestHandler<ObterFormacaoDetalhadaPorIdQuery, RetornoFormacaoDetalhadaDTO>
    {

        public async Task<RetornoFormacaoDetalhadaDTO> Handle(ObterFormacaoDetalhadaPorIdQuery request, CancellationToken cancellationToken)
        {
            var chaveRedis = CacheDistribuidoNomes.FormacaoDetalhada.Parametros(request.Id);
            var retornoFormacaoDetalhadaDto = await cacheDistribuido.ObterObjetoAsync<RetornoFormacaoDetalhadaDTO>(chaveRedis);

            // Recuperar o filtro do cache
            var chaveRedisFiltro = CacheDistribuidoNomes.FormacaoFiltro.Parametros(CacheFiltroFormacaoNomes.CHAVE_FILTRO_LISTAGEM_FORMACAO, contextoAplicacao.LoginUsuario ?? string.Empty);
            var filtroListagemFormacaoDTO = await cacheDistribuido.ObterObjetoAsync<FiltroListagemFormacaoDTO>(chaveRedisFiltro);

            // Se não houver filtro no cache, usar valores padrão
            if (filtroListagemFormacaoDTO == null)
                filtroListagemFormacaoDTO = new FiltroListagemFormacaoDTO();

            if (retornoFormacaoDetalhadaDto.EhNulo())
            {
                var formacaoDetalhada = await repositorioProposta.ObterFormacaoDetalhadaPorIdAsync(request.Id) ?? 
                                        throw new NegocioException(MensagemNegocio.FORMACAO_NAO_ENCONTRADA, HttpStatusCode.NotFound);

                retornoFormacaoDetalhadaDto = mapper.Map<RetornoFormacaoDetalhadaDTO>(formacaoDetalhada);

                var resultado = await ObterFormacoesSeguintesEAnteriorPorIdAsync(request.Id, new Infra.Dados.Dtos.FiltroListaFormacaoPropostaDto
                {
                    AreasPromotorasIds = filtroListagemFormacaoDTO.AreasPromotorasIds,
                    DataFinal = filtroListagemFormacaoDTO.DataFinal,
                    DataInicial = filtroListagemFormacaoDTO.DataInicial,
                    FiltrarPorPerfil = false,
                    FormatosIds = filtroListagemFormacaoDTO.FormatosIds,
                    Pagina = 1,
                    PalavrasChavesIds = filtroListagemFormacaoDTO.PalavrasChavesIds,
                    TamanhoPagina = 1000, 
                    PublicosAlvosIds = filtroListagemFormacaoDTO.PublicosAlvosIds,
                    RfServidor = string.Empty,
                    Titulo = filtroListagemFormacaoDTO.Titulo
                });

                retornoFormacaoDetalhadaDto.FormacaoAnteriorId = resultado.anteriorId;
                retornoFormacaoDetalhadaDto.FormacaoPosteriorId = resultado.posteriorId;

                if (formacaoDetalhada.ArquivoImagemDivulgacao is not null)
                    retornoFormacaoDetalhadaDto.ImagemUrl = await mediator.Send(new ObterEnderecoArquivoServicoArmazenamentoQuery(formacaoDetalhada.ArquivoImagemDivulgacao.NomeArquivoFisico, false), cancellationToken);

                await cacheDistribuido.SalvarAsync(chaveRedis, retornoFormacaoDetalhadaDto);
            }

            if (retornoFormacaoDetalhadaDto.FormacaoHomologada != Dominio.Enumerados.FormacaoHomologada.Sim)
            {
                var turmasComVaga = await repositorioProposta.ObterTurmasComVagaPorId(request.Id);
                foreach (var turma in retornoFormacaoDetalhadaDto.Turmas)
                {
                    turma.InscricaoEncerrada = !turmasComVaga.Any(t => t.Id == turma.Id);
                }
            }

            retornoFormacaoDetalhadaDto.InscricaoEncerrada = retornoFormacaoDetalhadaDto.DataInscricaoFim.Date < DateTimeExtension.HorarioBrasilia().Date || !retornoFormacaoDetalhadaDto.Turmas.Any(a => !a.InscricaoEncerrada);

            StringComparer numComparer = StringComparer.Create(CultureInfo.CurrentCulture, CompareOptions.NumericOrdering);
            retornoFormacaoDetalhadaDto.Turmas = [.. retornoFormacaoDetalhadaDto.Turmas.OrderBy(x => x.Nome, numComparer)];

            var acessibilidade = await repositorioUsuarioAcessibilidade.ObterAcessibilidadeAtualDoUsuarioAsync();
            retornoFormacaoDetalhadaDto.UsuarioAcessibilidade = mapper.Map<UsuarioAcessibilidadeDto>(acessibilidade);
            return retornoFormacaoDetalhadaDto;
        }

        private async Task<(long? anteriorId, long? posteriorId)> ObterFormacoesSeguintesEAnteriorPorIdAsync(long propostaId, Infra.Dados.Dtos.FiltroListaFormacaoPropostaDto filtro)
        {
            if (filtro == null)
                return (null, null);

            var resultado = await repositorioProposta.ObterListagemFormacoesPorFiltro(filtro);

            if (resultado?.Itens == null || !resultado.Itens.Any())
                return (null, null);

            var listaIds = resultado.Itens.ToList();
            var posicaoAtual = listaIds.IndexOf(propostaId);

            if (posicaoAtual == -1)
                return (null, null);

            var quantidadeItens = listaIds.Count;

            if (quantidadeItens == 1)
                return (null, null);

            var posicaoAnterior = posicaoAtual == 0 ? quantidadeItens - 1 : posicaoAtual - 1;
            var posicaoPosterior = posicaoAtual == quantidadeItens - 1 ? 0 : posicaoAtual + 1;

            return (listaIds[posicaoAnterior], listaIds[posicaoPosterior]);
        }
    }
}