using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterPropostasDashboard : CasoDeUsoAbstrato, ICasoDeUsoObterPropostasDashboard
    {
        private const int QUANTIDADE_MINIMA_PARA_EXIBIR_VERMAIS = 5;
        private readonly ICacheDistribuido _cacheDistribuido;

        public CasoDeUsoObterPropostasDashboard(IMediator mediator, ICacheDistribuido cacheDistribuido) : base(mediator)
        {
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
        }

        public async Task<IEnumerable<PropostaDashboardDTO>> Executar(PropostaFiltrosDashboardDTO filtro)
        {
            var listaRetorno = new List<PropostaDashboardDTO>();
            var listaDeSituacoesExistentes = Enum.GetValues(typeof(SituacaoProposta)).Cast<SituacaoProposta>();

            foreach (var situacao in listaDeSituacoesExistentes)
            {
                var propostasId = await mediator.Send(new ObterPropostasIdDashboardQuery(filtro, situacao));

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
                        var chaveCache = CacheDistribuidoNomes.DashboardProposta.Parametros(propostaId.Id);
                        var propostaItem = await _cacheDistribuido.ObterObjetoAsync<Dominio.Entidades.Proposta>(chaveCache);

                        if (propostaItem.EhNulo())
                            buscarPropostaBanco.Add(propostaId.Id);
                        else
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

                    if (buscarPropostaBanco.PossuiElementos())
                    {
                        var propostas = await mediator.Send(new ObterPropostasDashboardQuery(buscarPropostaBanco.ToArray()));

                        foreach (var proposta in propostas)
                        {
                            var dataFormatada = (proposta.Movimentacao?.CriadoEm ?? proposta?.AlteradoEm ?? proposta!.CriadoEm).ToString("g");
                            var itemProposta = new PropostaDashboardItemDTO
                            {
                                Numero = proposta.Id,
                                Nome = proposta.NomeFormacao,
                                Data = dataFormatada
                            };

                            item.Propostas.Add(itemProposta);

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
    }
}