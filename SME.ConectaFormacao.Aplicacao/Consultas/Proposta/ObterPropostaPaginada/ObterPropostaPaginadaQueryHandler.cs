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
        private readonly IMediator _mediator;

        public ObterPropostaPaginadaQueryHandler(IMapper mapper, IRepositorioProposta repositorioProposta, IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<PaginacaoResultadoDTO<PropostaPaginadaDTO>> Handle(ObterPropostaPaginadaQuery request, CancellationToken cancellationToken)
        {
            var usuarioLogado = await _mediator.Send(new ObterUsuarioLogadoQuery());
            var perfilUsuarioLogado = await _mediator.Send(new ObterGrupoUsuarioLogadoQuery());

            var totalRegistrosFiltro = await _repositorioProposta.ObterTotalRegistrosPorFiltros(
                request.AreaPromotoraIdUsuarioLogado,
                request.PropostaFiltrosDTO.Id,
                request.PropostaFiltrosDTO.AreaPromotoraId,
                request.PropostaFiltrosDTO.Formato,
                request.PropostaFiltrosDTO.PublicoAlvoIds,
                request.PropostaFiltrosDTO.NomeFormacao,
                request.PropostaFiltrosDTO.NumeroHomologacao,
                request.PropostaFiltrosDTO.PeriodoRealizacaoInicio,
                request.PropostaFiltrosDTO.PeriodoRealizacaoFim,
                request.PropostaFiltrosDTO.Situacao,
                request.PropostaFiltrosDTO.FormacaoHomologada,
                usuarioLogado.Login,
                perfilUsuarioLogado);

            IEnumerable<Proposta> propostas = new List<Proposta>();
            if (totalRegistrosFiltro > 0)
            {
                propostas = await _repositorioProposta.ObterDadosPaginados(
                    request.AreaPromotoraIdUsuarioLogado,
                    request.NumeroPagina,
                    request.NumeroRegistros,
                    request.PropostaFiltrosDTO.Id,
                    request.PropostaFiltrosDTO.AreaPromotoraId,
                    request.PropostaFiltrosDTO.Formato,
                    request.PropostaFiltrosDTO.PublicoAlvoIds,
                    request.PropostaFiltrosDTO.NomeFormacao,
                    request.PropostaFiltrosDTO.NumeroHomologacao,
                    request.PropostaFiltrosDTO.PeriodoRealizacaoInicio,
                    request.PropostaFiltrosDTO.PeriodoRealizacaoFim,
                    request.PropostaFiltrosDTO.Situacao,
                    request.PropostaFiltrosDTO.FormacaoHomologada,
                    usuarioLogado.Login,
                    perfilUsuarioLogado);
            }

            var items = _mapper.Map<IEnumerable<PropostaPaginadaDTO>>(propostas);
            return new PaginacaoResultadoDTO<PropostaPaginadaDTO>(items, totalRegistrosFiltro, request.NumeroRegistros);
        }
    }
}
