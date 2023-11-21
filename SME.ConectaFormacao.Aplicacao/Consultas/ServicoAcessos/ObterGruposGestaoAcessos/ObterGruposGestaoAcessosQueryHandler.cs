using System.Collections;
using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Grupo;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterGruposGestaoAcessosQueryHandler : IRequestHandler<ObterGruposGestaoAcessosQuery, IEnumerable<GrupoDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioGrupoGestao _repositorioGrupoGestao;

        public ObterGruposGestaoAcessosQueryHandler(IMapper mapper, IRepositorioGrupoGestao repositorioGrupoGestao)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioGrupoGestao = repositorioGrupoGestao ?? throw new ArgumentNullException(nameof(repositorioGrupoGestao));
        }

        public async Task<IEnumerable<GrupoDTO>> Handle(ObterGruposGestaoAcessosQuery request, CancellationToken cancellationToken)
        {
            return (await _repositorioGrupoGestao.ObterTodos()).Select(s => new GrupoDTO() { Id = s.GrupoId, Nome = s.Nome });
        }
    }
}
