using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPerfilAreaPromotoraQueryHandler : IRequestHandler<ObterPerfilAreaPromotoraQuery, RetornoListagemDTO>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioAreaPromotora _repositorioAreaPromotora;
        private readonly ICacheDistribuido _cacheDistribuido;

        public ObterPerfilAreaPromotoraQueryHandler(IMapper mapper, IRepositorioAreaPromotora repositorioAreaPromotora, ICacheDistribuido cacheDistribuido)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioAreaPromotora = repositorioAreaPromotora ?? throw new ArgumentNullException(nameof(repositorioAreaPromotora));
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
        }

        public async Task<RetornoListagemDTO> Handle(ObterPerfilAreaPromotoraQuery request, CancellationToken cancellationToken)
        {
            var areasPromotoras = await _cacheDistribuido.ObterAsync(CacheDistribuidoNomes.AreaPromotora, () => _repositorioAreaPromotora.ObterLista());

            return _mapper.Map<RetornoListagemDTO>(areasPromotoras.FirstOrDefault(f => f.GrupoId == request.GrupoId));
        }
    }
}
