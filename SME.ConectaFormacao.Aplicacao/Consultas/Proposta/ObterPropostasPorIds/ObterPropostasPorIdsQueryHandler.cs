using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
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
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly IMapper _mapper;
        private readonly ICacheDistribuido _cacheDistribuido;
        private readonly IConfiguration _configuration;
        
        public ObterPropostasPorIdsQueryHandler(IRepositorioProposta repositorioProposta, IMapper mapper,ICacheDistribuido cacheDistribuido, IConfiguration configuration)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        
        public async Task<IEnumerable<RetornoListagemFormacaoDTO>> Handle(ObterPropostasPorIdsQuery request, CancellationToken cancellationToken)
        {
            var propostasResumidas = new List<PropostaResumida>();
            foreach (var propostaId in request.PropostasIds)
            {
                var chaveRedis = string.Format(CacheDistribuidoNomes.PropostaId, propostaId);
                var proposta = await _cacheDistribuido.ObterAsync(chaveRedis, () => _repositorioProposta.ObterPropostaPorId(propostaId));
                proposta.ImagemUrl = ObterImagemUrl(proposta);
                propostasResumidas.Add(proposta);  
            }
            return _mapper.Map<IEnumerable<RetornoListagemFormacaoDTO>>(propostasResumidas);
        }

        private string ObterImagemUrl(PropostaResumida proposta)
        {
            if (proposta.NomeArquivo.EhNulo())
                return default;
            
            var extensao = Path.GetExtension(proposta.NomeArquivo);
            
            var nomeArquivoComExtensao = $"{proposta.CodigoArquivo}{extensao}";
            
            return $"{_configuration["UrlFrontEnd"]}conecta/{nomeArquivoComExtensao}";
        }
    }
}
