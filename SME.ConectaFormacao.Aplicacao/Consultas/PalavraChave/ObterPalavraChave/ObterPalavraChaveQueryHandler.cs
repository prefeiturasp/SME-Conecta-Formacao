using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPalavraChaveQueryHandler : IRequestHandler<ObterPalavraChaveQuery, IEnumerable<RetornoListagemDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioPalavraChave _repositorioPalavraChave;
        private readonly ICacheDistribuido _cacheDistribuido;

        public ObterPalavraChaveQueryHandler(IMapper mapper, IRepositorioPalavraChave repositorioPalavraChave, ICacheDistribuido cacheDistribuido)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioPalavraChave = repositorioPalavraChave ?? throw new ArgumentNullException(nameof(repositorioPalavraChave));
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
        }

        public async Task<IEnumerable<RetornoListagemDTO>> Handle(ObterPalavraChaveQuery request, CancellationToken cancellationToken)
        {
            var palavrasChaves = await _cacheDistribuido.ObterAsync(CacheDistribuidoNomes.PalavraChave, () => _repositorioPalavraChave.ObterLista());
            return _mapper.Map<IEnumerable<RetornoListagemDTO>>(palavrasChaves);
        }
    }
}
