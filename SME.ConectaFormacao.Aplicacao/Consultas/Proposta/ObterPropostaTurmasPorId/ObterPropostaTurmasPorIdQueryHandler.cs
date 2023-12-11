using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaTurmasPorIdQueryHandler : IRequestHandler<ObterPropostaTurmasPorIdQuery, IEnumerable<RetornoListagemDTO>>
    {
        private readonly IMapper _mapper;
        public readonly IRepositorioProposta _repositorioProposta;

        public ObterPropostaTurmasPorIdQueryHandler(IMapper mapper, IRepositorioProposta repositorioProposta)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<IEnumerable<RetornoListagemDTO>> Handle(ObterPropostaTurmasPorIdQuery request, CancellationToken cancellationToken)
        {
            var proposta = await _repositorioProposta.ObterPorId(request.Id) ??
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA, System.Net.HttpStatusCode.NotFound);

            var turmas = await _repositorioProposta.ObterTurmasPorId(request.Id);
            return _mapper.Map<IEnumerable<RetornoListagemDTO>>(turmas);
        }
    }
}
