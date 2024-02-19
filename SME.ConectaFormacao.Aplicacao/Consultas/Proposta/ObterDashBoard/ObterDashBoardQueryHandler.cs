using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterDashBoardQueryHandler : IRequestHandler<ObterDashBoardQuery, IEnumerable<PropostaDashboardDTO>>
    {
        private const int QUANTIDADE_MINIMA_PARA_EXIBIR_VERMAIS = 5;

        private readonly IMediator _mediator;
        private readonly ICacheDistribuido _cacheDistribuido;

        public ObterDashBoardQueryHandler(IMediator mediator, ICacheDistribuido cacheDistribuido)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
        }

        public async Task<IEnumerable<PropostaDashboardDTO>> Handle(ObterDashBoardQuery request, CancellationToken cancellationToken)
        {
            var listaRetorno = new List<PropostaDashboardDTO>();
            var listaDeSituacoesExistentes = Enum.GetValues(typeof(SituacaoProposta)).Cast<SituacaoProposta>();

            if (request.PropostaFiltrosDashboardDTO.Situacao.HasValue)
                listaDeSituacoesExistentes = listaDeSituacoesExistentes.Where(t => t == request.PropostaFiltrosDashboardDTO.Situacao.Value);

            var propostas = await _mediator.Send(new ObterPropostasIdDashboardQuery(request.PropostaFiltrosDashboardDTO, listaDeSituacoesExistentes, request.AreaPromotoraIdUsuarioLogado), cancellationToken);

            foreach (var situacao in listaDeSituacoesExistentes)
            {
                var propostasId = propostas.Where(w => w.Situacao == situacao).Select(t => t.Id);
                var total = propostasId.Count();

                var item = new PropostaDashboardDTO
                {
                    Situacao = situacao,
                    Cor = situacao.Cor(),
                    TotalRegistros = total > QUANTIDADE_MINIMA_PARA_EXIBIR_VERMAIS ? (total - QUANTIDADE_MINIMA_PARA_EXIBIR_VERMAIS).ToString() : string.Empty,
                };

                if (propostasId.PossuiElementos())
                {
                    var buscarPropostaBanco = new List<long>();
                    foreach (var propostaId in propostasId.Take(QUANTIDADE_MINIMA_PARA_EXIBIR_VERMAIS))
                    {
                        var chaveCache = CacheDistribuidoNomes.DashboardProposta.Parametros(propostaId);
                        var propostaItem = await _cacheDistribuido.ObterObjetoAsync<Proposta>(chaveCache);

                        if (propostaItem.EhNulo())
                        {
                            buscarPropostaBanco.Add(propostaId);
                            continue;
                        }

                        MapearProposta(item, propostaItem);
                    }

                    if (buscarPropostaBanco.PossuiElementos())
                    {
                        var propostasCompleta = await _mediator.Send(new ObterPropostasDashboardQuery(buscarPropostaBanco.ToArray()), cancellationToken);

                        foreach (var proposta in propostasCompleta)
                        {
                            MapearProposta(item, proposta);

                            var chaveCache = CacheDistribuidoNomes.DashboardProposta.Parametros(proposta.Id);
                            await _cacheDistribuido.SalvarAsync(chaveCache, proposta);
                        }
                    }
                }
                if (item.Propostas.PossuiElementos())
                    listaRetorno.Add(item);
            }

            return listaRetorno;
        }

        private static void MapearProposta(PropostaDashboardDTO item, Proposta propostaItem)
        {
            var dataFormatada = (propostaItem.Movimentacao?.CriadoEm ?? propostaItem?.AlteradoEm ?? propostaItem!.CriadoEm).ToString("dd/MM/yyyy HH:mm");
            var itemProposta = new PropostaDashboardItemDTO
            {
                Numero = propostaItem.Id,
                Nome = propostaItem.NomeFormacao,
                Data = dataFormatada
            };

            item.Propostas.Add(itemProposta);
        }
    }
}
