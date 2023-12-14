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
    public class ObterPropostaDetalhePorIdQueryHandler : IRequestHandler<ObterPropostaDetalhePorIdQuery, RetornoDetalheFormacaoDTO>
    {
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ICacheDistribuido _cacheDistribuido;

        public ObterPropostaDetalhePorIdQueryHandler(IRepositorioProposta repositorioProposta,IMapper mapper,IMediator mediator, ICacheDistribuido cacheDistribuido)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
        }

        public async Task<RetornoDetalheFormacaoDTO> Handle(ObterPropostaDetalhePorIdQuery request, CancellationToken cancellationToken)
        {
            var chaveRedis = CacheDistribuidoNomes.FormacaoDetalhada.Parametros(request.Id);
            var retornoDetalheFormacaoDto = await _cacheDistribuido.ObterObjetoAsync<RetornoDetalheFormacaoDTO>(chaveRedis);

            if (retornoDetalheFormacaoDto.EhNulo())
            {
                var detalheFormacao = await _repositorioProposta.ObterFormacaoDetalhadaPorId(request.Id);
                
                var retornoDetalheFormacao = _mapper.Map<RetornoDetalheFormacaoDTO>(detalheFormacao);
                
                retornoDetalheFormacao.ImagemUrl = await _mediator.Send(new ObterEnderecoArquivoServicoArmazenamentoQuery(retornoDetalheFormacao.ImagemUrl, false));
                
                await _cacheDistribuido.SalvarAsync(chaveRedis, retornoDetalheFormacao);
                
                return retornoDetalheFormacao;
            }

            return retornoDetalheFormacaoDto;
        }
    }
}