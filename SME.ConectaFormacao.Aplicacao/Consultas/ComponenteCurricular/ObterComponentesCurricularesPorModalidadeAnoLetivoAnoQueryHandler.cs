using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Base;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterComponentesCurricularesPorModalidadeAnoLetivoAnoQueryHandler : IRequestHandler<ObterComponentesCurricularesPorModalidadeAnoLetivoAnoQuery, IEnumerable<IdNomeTodosDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioComponenteCurricular _repositorioComponenteCurricular;
        
        public ObterComponentesCurricularesPorModalidadeAnoLetivoAnoQueryHandler(IMapper mapper, IRepositorioComponenteCurricular repositorioComponenteCurricular)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioComponenteCurricular = repositorioComponenteCurricular ?? throw new ArgumentNullException(nameof(repositorioComponenteCurricular));
        }

        public async Task<IEnumerable<IdNomeTodosDTO>> Handle(ObterComponentesCurricularesPorModalidadeAnoLetivoAnoQuery request, CancellationToken cancellationToken)
        {
            var anos = await _repositorioComponenteCurricular.ObterComponentesCurricularesPorModalidadeAnoLetivoAno(request.Modalidade, request.AnoLetivo, request.Ano);
            return _mapper.Map<IEnumerable<IdNomeTodosDTO>>(anos);
        }
    }
}
