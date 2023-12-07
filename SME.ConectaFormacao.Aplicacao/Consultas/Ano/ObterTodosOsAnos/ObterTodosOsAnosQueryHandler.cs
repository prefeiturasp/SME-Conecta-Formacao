using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTodosOsAnosQueryHandler : IRequestHandler<ObterTodosOsAnosQuery, IEnumerable<AnoTurma>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioAnoTurma _repositorioAnoTurma;
        
        public ObterTodosOsAnosQueryHandler(IMapper mapper, IRepositorioAnoTurma repositorioAnoTurma)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioAnoTurma = repositorioAnoTurma ?? throw new ArgumentNullException(nameof(repositorioAnoTurma));
        }

        public async Task<IEnumerable<AnoTurma>> Handle(ObterTodosOsAnosQuery request, CancellationToken cancellationToken)
        {
            return await _repositorioAnoTurma.ObterTodos();
        }
    }
}
