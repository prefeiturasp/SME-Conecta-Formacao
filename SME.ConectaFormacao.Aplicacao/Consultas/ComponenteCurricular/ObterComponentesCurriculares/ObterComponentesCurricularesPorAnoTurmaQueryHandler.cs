using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterComponentesCurricularesPorAnoTurmaQueryHandler : IRequestHandler<ObterComponentesCurricularesPorAnoTurmaQuery, IEnumerable<RetornoListagemTodosDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioComponenteCurricular _repositorioComponenteCurricular;

        public ObterComponentesCurricularesPorAnoTurmaQueryHandler(IMapper mapper, IRepositorioComponenteCurricular repositorioComponenteCurricular)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioComponenteCurricular = repositorioComponenteCurricular ?? throw new ArgumentNullException(nameof(repositorioComponenteCurricular));
        }

        public async Task<IEnumerable<RetornoListagemTodosDTO>> Handle(ObterComponentesCurricularesPorAnoTurmaQuery request, CancellationToken cancellationToken)
        {
            var anosTurma = await _repositorioComponenteCurricular.ObterComponentesCurricularesPorAnoTurma(request.AnoTurmaId, request.ExibirOpcaoTodos);
            return _mapper.Map<IEnumerable<RetornoListagemTodosDTO>>(anosTurma);
        }
    }
}
