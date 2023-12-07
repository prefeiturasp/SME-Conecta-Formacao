using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Base;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterComponentesCurricularesEAnoTurmaPorModalidadeAnoLetivoAnoTurmaIdQueryHandler : IRequestHandler<ObterComponentesCurricularesEAnoTurmaPorModalidadeAnoIdLetivoAnoTurmaQuery, IEnumerable<RetornoListagemTodosDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioComponenteCurricular _repositorioComponenteCurricular;
        
        public ObterComponentesCurricularesEAnoTurmaPorModalidadeAnoLetivoAnoTurmaIdQueryHandler(IMapper mapper, IRepositorioComponenteCurricular repositorioComponenteCurricular)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioComponenteCurricular = repositorioComponenteCurricular ?? throw new ArgumentNullException(nameof(repositorioComponenteCurricular));
        }

        public async Task<IEnumerable<RetornoListagemTodosDTO>> Handle(ObterComponentesCurricularesEAnoTurmaPorModalidadeAnoIdLetivoAnoTurmaQuery request, CancellationToken cancellationToken)
        {
            var anosTurma = await _repositorioComponenteCurricular.ObterComponentesCurricularesPorModalidadeAnoLetivoAno(request.Modalidade, request.AnoLetivo, request.AnoTurmaId);
            return _mapper.Map<IEnumerable<RetornoListagemTodosDTO>>(anosTurma);
        }
    }
}
