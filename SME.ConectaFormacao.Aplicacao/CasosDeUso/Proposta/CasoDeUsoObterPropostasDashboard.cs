using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterPropostasDashboard : CasoDeUsoAbstrato,ICasoDeUsoObterPropostasDashboard
    {
        private readonly IMapper _mapper;
        private const int QUANTIDADE_MINIMA_PARA_EXIBIR_VERMAIS = 5;

        public CasoDeUsoObterPropostasDashboard(IMediator mediator, IMapper mapper) : base(mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<PropostaDashboardDTO>> Executar(PropostaFiltrosDashboardDTO filtro)
        {
            var listaRetorno = new List<PropostaDashboardDTO>();

            var propostasId = await mediator.Send(new ObterPropostasIdDashboardQuery(filtro));

            if (propostasId.PossuiElementos())
            {
                var listaDePropostasNaBase = await mediator.Send(new ObterPropostasDashboardQuery(propostasId));
                var totalPorTipo = await mediator.Send(new ObterTotalDashboardPorTipoQuery(filtro));
                var listaDeSituacoesExistentes =  Enum.GetValues(typeof(SituacaoProposta)).Cast<SituacaoProposta>();

                foreach (var situacao in listaDeSituacoesExistentes)
                {
                    var propostas = listaDePropostasNaBase.Where(x => x.Situacao == situacao);
                    if (propostas.Any())
                    {
                        var total = totalPorTipo.FirstOrDefault(x => x.Situacao == situacao).Quantidade;
                        var item = new PropostaDashboardDTO
                        {
                            Situacao = situacao,
                            Cor = situacao.Cor(),
                            TotalRegistros = total > QUANTIDADE_MINIMA_PARA_EXIBIR_VERMAIS ? (total - QUANTIDADE_MINIMA_PARA_EXIBIR_VERMAIS).ToString() : string.Empty,
                        };
                        foreach (var propostaItem in propostas)
                        {
                            var dataFormatada = (propostaItem.Movimentacao?.CriadoEm ?? propostaItem?.AlteradoEm ?? propostaItem!.CriadoEm).ToString("g");
                            var itemProposta = new PropostaDashboardItemDTO
                            {
                                Numero = propostaItem.Id,
                                Nome = propostaItem.NomeFormacao,
                                Data = dataFormatada
                            };
                            item.Propostas.Add(itemProposta);
                        }

                        listaRetorno.Add(item);
                    }
                }
            }
            return listaRetorno;
        }
    }
}