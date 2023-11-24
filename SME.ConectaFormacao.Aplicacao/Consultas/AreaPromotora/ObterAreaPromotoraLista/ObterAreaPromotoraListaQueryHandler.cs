using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAreaPromotoraListaQueryHandler : IRequestHandler<ObterAreaPromotoraListaQuery, IEnumerable<RetornoListagemDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioAreaPromotora _repositorioAreaPromotora;

        public ObterAreaPromotoraListaQueryHandler(IMapper mapper, IRepositorioAreaPromotora repositorioAreaPromotora)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioAreaPromotora = repositorioAreaPromotora ?? throw new ArgumentNullException(nameof(repositorioAreaPromotora));
        }

        public async Task<IEnumerable<RetornoListagemDTO>> Handle(ObterAreaPromotoraListaQuery request, CancellationToken cancellationToken)
        {
            var areasPromotoras = await _repositorioAreaPromotora.ObterLista(request.GrupoId, request.DresCodigo);
            return _mapper.Map<IEnumerable<RetornoListagemDTO>>(areasPromotoras);
        }
    }
}
