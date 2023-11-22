using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Grupo;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterGruposGestaoQueryHandler : IRequestHandler<ObterGruposGestaoQuery, IEnumerable<GrupoDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioGrupoGestao _repositorioGrupoGestao;

        public ObterGruposGestaoQueryHandler(IMapper mapper, IRepositorioGrupoGestao repositorioGrupoGestao)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioGrupoGestao = repositorioGrupoGestao ?? throw new ArgumentNullException(nameof(repositorioGrupoGestao));
        }

        public async Task<IEnumerable<GrupoDTO>> Handle(ObterGruposGestaoQuery request, CancellationToken cancellationToken)
        {
            var gruposGestao = await _repositorioGrupoGestao.ObterTodos(); 
            return _mapper.Map<IEnumerable<GrupoDTO>>(gruposGestao.AsEnumerable());
        }
    }
}
