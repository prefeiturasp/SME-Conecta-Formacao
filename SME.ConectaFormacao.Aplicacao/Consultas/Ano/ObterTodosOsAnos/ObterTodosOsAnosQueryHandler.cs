using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTodosOsAnosQueryHandler : IRequestHandler<ObterTodosOsAnosQuery, IEnumerable<Ano>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioAno _repositorioAno;
        
        public ObterTodosOsAnosQueryHandler(IMapper mapper, IRepositorioAno repositorioAno)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioAno = repositorioAno ?? throw new ArgumentNullException(nameof(repositorioAno));
        }

        public async Task<IEnumerable<Ano>> Handle(ObterTodosOsAnosQuery request, CancellationToken cancellationToken)
        {
            return await _repositorioAno.ObterTodos();
        }
    }
}
