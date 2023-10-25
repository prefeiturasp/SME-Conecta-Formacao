using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Proposta.ObterRegentesPaginado
{
    public class ObterRegentesPaginadoQueryHandler : IRequestHandler<ObterRegentesPaginadoQuery, PaginacaoResultadoDTO<PropostaRegenteDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterRegentesPaginadoQueryHandler(IMapper mapper, IRepositorioProposta repositorioProposta)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<PaginacaoResultadoDTO<PropostaRegenteDTO>> Handle(ObterRegentesPaginadoQuery request, CancellationToken cancellationToken)
        {
            var totalRegistros = await _repositorioProposta.ObterTotalRegentes(request.PropostaId);
            IEnumerable<PropostaRegente> regentes = new List<PropostaRegente>();
            if (totalRegistros > 0)
            {
                regentes = await _repositorioProposta.ObterRegentesPaginado(request.NumeroPagina, request.NumeroRegistros, request.PropostaId);
                var ids = regentes.Select(t => t.Id).ToArray();
                var turmas = await _repositorioProposta.ObterRegenteTurmasPorRegenteId(ids);
                foreach (var regente in regentes)
                    regente.Turmas = turmas.Where(x => x.PropostaRegenteId == regente.Id);
            }
            var items = _mapper.Map<IEnumerable<PropostaRegenteDTO>>(regentes);
            return new PaginacaoResultadoDTO<PropostaRegenteDTO>(items, totalRegistros, request.NumeroRegistros);
        }
    }
}