using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterInscricaoPaginadaPorUsuarioIdQueryHandler : IRequestHandler<ObterInscricaoPaginadaPorUsuarioIdQuery, PaginacaoResultadoDTO<InscricaoPaginadaDTO>>
    {
        private readonly IRepositorioInscricao _repositorioInscricao;
        private readonly IMapper _mapper;

        public ObterInscricaoPaginadaPorUsuarioIdQueryHandler(IRepositorioInscricao repositorioInscricao, IMapper mapper)
        {
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PaginacaoResultadoDTO<InscricaoPaginadaDTO>> Handle(ObterInscricaoPaginadaPorUsuarioIdQuery request, CancellationToken cancellationToken)
        {
            var totalRegistrosFiltro = await _repositorioInscricao.ObterTotalRegistrosPorUsuarioId(request.UsuarioId);

            var registros = Enumerable.Empty<Dominio.Entidades.Inscricao>();
            if (totalRegistrosFiltro > 0)
            {
                registros = await _repositorioInscricao.ObterDadosPaginadosPorUsuarioId(request.UsuarioId, request.NumeroPagina, request.NumeroRegistros);
            }

            var items = _mapper.Map<IEnumerable<InscricaoPaginadaDTO>>(registros);
            return new PaginacaoResultadoDTO<InscricaoPaginadaDTO>(items, totalRegistrosFiltro, request.NumeroRegistros);
        }
    }
}
