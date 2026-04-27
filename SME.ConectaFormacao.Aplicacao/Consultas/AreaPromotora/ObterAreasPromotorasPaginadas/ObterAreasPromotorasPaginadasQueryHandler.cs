using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAreasPromotorasPaginadasQueryHandler : IRequestHandler<ObterAreasPromotorasPaginadasQuery, PaginacaoResultadoDto<AreaPromotoraPaginadaDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioAreaPromotora _repositorioAreaPromotora;

        public ObterAreasPromotorasPaginadasQueryHandler(IMapper mapper, IRepositorioAreaPromotora repositorioAreaPromotora)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioAreaPromotora = repositorioAreaPromotora ?? throw new ArgumentNullException(nameof(repositorioAreaPromotora));
        }

        public async Task<PaginacaoResultadoDto<AreaPromotoraPaginadaDTO>> Handle(ObterAreasPromotorasPaginadasQuery request, CancellationToken cancellationToken)
        {
            var totalRegistrosFiltro = await _repositorioAreaPromotora.ObterTotalRegistrosPorFiltros(request.Filtros.Nome, request.Filtros.Tipo, request.Filtros.CoordenadoriaId);

            IEnumerable<AreaPromotora> areasPromotoras = new List<AreaPromotora>();
            if (totalRegistrosFiltro > 0)
                areasPromotoras = await _repositorioAreaPromotora.ObterDadosPaginados(request.Filtros.Nome, request.Filtros.Tipo, request.Filtros.CoordenadoriaId, request.NumeroPagina, request.NumeroRegistros);

            var items = _mapper.Map<IEnumerable<AreaPromotoraPaginadaDTO>>(areasPromotoras);
            return new PaginacaoResultadoDto<AreaPromotoraPaginadaDTO>(items, totalRegistrosFiltro, request.NumeroRegistros);
        }
    }
}
