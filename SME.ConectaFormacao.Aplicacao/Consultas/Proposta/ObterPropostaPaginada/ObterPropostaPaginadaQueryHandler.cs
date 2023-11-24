using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaPaginadaQueryHandler : IRequestHandler<ObterPropostaPaginadaQuery, PaginacaoResultadoDTO<PropostaPaginadaDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterPropostaPaginadaQueryHandler(IMapper mapper, IRepositorioProposta repositorioProposta)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<PaginacaoResultadoDTO<PropostaPaginadaDTO>> Handle(ObterPropostaPaginadaQuery request, CancellationToken cancellationToken)
        {
            var totalRegistrosFiltro = await _repositorioProposta.ObterTotalRegistrosPorFiltros(
                request.PropostaFiltrosDTO.Id,
                request.PropostaFiltrosDTO.AreaPromotoraId,
                request.PropostaFiltrosDTO.Modalidade,
                request.PropostaFiltrosDTO.PublicoAlvoIds,
                request.PropostaFiltrosDTO.NomeFormacao,
                request.PropostaFiltrosDTO.NumeroHomologacao,
                request.PropostaFiltrosDTO.PeriodoRealizacaoInicio,
                request.PropostaFiltrosDTO.PeriodoRealizacaoFim,
                request.PropostaFiltrosDTO.Situacao,
                request.PropostaFiltrosDTO.FormacaoHomologada);

            IEnumerable<Proposta> propostas = new List<Proposta>();
            if (totalRegistrosFiltro > 0)
            {
                propostas = await _repositorioProposta.ObterDadosPaginados(
                request.NumeroPagina,
                request.NumeroRegistros,
                request.PropostaFiltrosDTO.Id,
                request.PropostaFiltrosDTO.AreaPromotoraId,
                request.PropostaFiltrosDTO.Modalidade,
                request.PropostaFiltrosDTO.PublicoAlvoIds,
                request.PropostaFiltrosDTO.NomeFormacao,
                request.PropostaFiltrosDTO.NumeroHomologacao,
                request.PropostaFiltrosDTO.PeriodoRealizacaoInicio,
                request.PropostaFiltrosDTO.PeriodoRealizacaoFim,
                request.PropostaFiltrosDTO.Situacao,
                request.PropostaFiltrosDTO.FormacaoHomologada
                );
            }

            var items = _mapper.Map<IEnumerable<PropostaPaginadaDTO>>(propostas);
            return new PaginacaoResultadoDTO<PropostaPaginadaDTO>(items, totalRegistrosFiltro, request.NumeroRegistros);
        }
    }
}
