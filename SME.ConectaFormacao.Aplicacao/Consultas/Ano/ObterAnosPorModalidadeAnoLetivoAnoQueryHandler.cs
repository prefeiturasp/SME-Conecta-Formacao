using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Base;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAnosPorModalidadeAnoLetivoAnoQueryHandler : IRequestHandler<ObterComponentesCurricularesPorModalidadeAnoLetivoAnoQuery, IEnumerable<IdNomeOutrosDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioComponenteCurricular _repositorioComponenteCurricular;
        
        public ObterAnosPorModalidadeAnoLetivoAnoQueryHandler(IMapper mapper, IRepositorioComponenteCurricular repositorioComponenteCurricular)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioComponenteCurricular = repositorioComponenteCurricular ?? throw new ArgumentNullException(nameof(repositorioComponenteCurricular));
        }

        public async Task<IEnumerable<IdNomeOutrosDTO>> Handle(ObterComponentesCurricularesPorModalidadeAnoLetivoAnoQuery request, CancellationToken cancellationToken)
        {
            var componenteCurriculares = (await _repositorioComponenteCurricular.ObterTodos()).Where(w => !w.Excluido);
            return _mapper.Map<IEnumerable<IdNomeOutrosDTO>>(componenteCurriculares);
        }
    }
}
