using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAnosPorModalidadeAnoLetivoQueryHandler : IRequestHandler<ObterAnosPorModalidadeAnoLetivoQuery, IEnumerable<RetornoListagemTodosDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioAnoTurma _repositorioAnoTurma;
        
        public ObterAnosPorModalidadeAnoLetivoQueryHandler(IMapper mapper, IRepositorioAnoTurma repositorioAnoTurma)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioAnoTurma = repositorioAnoTurma ?? throw new ArgumentNullException(nameof(repositorioAnoTurma));
        }

        public async Task<IEnumerable<RetornoListagemTodosDTO>> Handle(ObterAnosPorModalidadeAnoLetivoQuery request, CancellationToken cancellationToken)
        {
            var anos = await _repositorioAnoTurma.ObterAnosPorModalidadeAnoLetivo(request.Modalidade, request.AnoLetivo, request.ExibirTodos);
            return _mapper.Map<IEnumerable<RetornoListagemTodosDTO>>(anos);
        }
    }
}
