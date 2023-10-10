using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.CriterioCertificacao.ObterCriterioCertificacao
{
    public class ObterCriterioCertificacaoQueryHandler : IRequestHandler<ObterCriterioCertificacaoQuery, IEnumerable<RetornoListagemDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioCriterioCertificacao _repositorio;

        public ObterCriterioCertificacaoQueryHandler(IMapper mapper,IRepositorioCriterioCertificacao repositorio)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
        }

        public async Task<IEnumerable<RetornoListagemDTO>> Handle(ObterCriterioCertificacaoQuery request, CancellationToken cancellationToken)
        {
            var criterios = (await _repositorio.ObterTodos()).Where(x => !x.Excluido);
            return _mapper.Map<IEnumerable<RetornoListagemDTO>>(criterios);
        }
    }
}