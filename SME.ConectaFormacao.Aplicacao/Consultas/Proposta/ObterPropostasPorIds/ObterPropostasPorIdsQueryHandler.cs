using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostasPorIdsQueryHandler : IRequestHandler<ObterPropostasPorIdsQuery, IEnumerable<RetornoListagemFormacaoDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly ICacheDistribuido _cacheDistribuido;
        private readonly IMediator _mediator;

        public ObterPropostasPorIdsQueryHandler(IMapper mapper, IRepositorioProposta repositorioProposta, ICacheDistribuido cacheDistribuido, IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<RetornoListagemFormacaoDTO>> Handle(ObterPropostasPorIdsQuery request, CancellationToken cancellationToken)
        {
            var propostas = new List<Proposta>();
            var buscarNoBanco = new List<long>();
            foreach (var propostaId in request.PropostasIds)
            {
                var chaveRedis = CacheDistribuidoNomes.FormacaoResumida.Parametros(propostaId);
                var proposta = await _cacheDistribuido.ObterObjetoAsync<Proposta>(chaveRedis);

                if (proposta == null)
                    buscarNoBanco.Add(propostaId);
                else
                    propostas.Add(proposta);
            }

            propostas.AddRange(await BuscarPropostasNoBanco(buscarNoBanco));

            return await MapearPropostaParaFormacao(propostas);
        }

        private async Task<IEnumerable<Proposta>> BuscarPropostasNoBanco(List<long> buscarNoBanco)
        {
            var propostasBanco = Enumerable.Empty<Proposta>();
            if (buscarNoBanco.PossuiElementos())
            {
                propostasBanco = await _repositorioProposta.ObterPropostaResumidaPorId(buscarNoBanco.ToArray());
                foreach (var propostaBanco in propostasBanco)
                {
                    var chaveRedis = CacheDistribuidoNomes.FormacaoResumida.Parametros(propostaBanco.Id);

                    await _cacheDistribuido.SalvarAsync(chaveRedis, propostaBanco);
                }
            }

            return propostasBanco;
        }

        private async Task<IEnumerable<RetornoListagemFormacaoDTO>> MapearPropostaParaFormacao(List<Proposta> propostas)
        {
            var formacoes = new List<RetornoListagemFormacaoDTO>();
            foreach (var proposta in propostas)
            {
                var formacao = _mapper.Map<RetornoListagemFormacaoDTO>(proposta);
                if (proposta.ArquivoImagemDivulgacao.NaoEhNulo())
                    formacao.ImagemUrl = await _mediator.Send(new ObterEnderecoArquivoServicoArmazenamentoQuery(proposta.ArquivoImagemDivulgacao.NomeArquivoFisico, false));

                formacoes.Add(formacao);
            }

            return formacoes;
        }
    }
}
