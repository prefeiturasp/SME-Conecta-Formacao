using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterGruposServicoAcessosQueryHandler : IRequestHandler<ObterGruposServicoAcessosQuery, IEnumerable<GrupoDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IServicoAcessos _servicoAcessos;

        public ObterGruposServicoAcessosQueryHandler(IMapper mapper, IServicoAcessos servicoAcessos)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
        }

        public async Task<IEnumerable<GrupoDTO>> Handle(ObterGruposServicoAcessosQuery request, CancellationToken cancellationToken)
        {
            var grupos = await _servicoAcessos.ObterGrupos();
            return _mapper.Map<IEnumerable<GrupoDTO>>(grupos);
        }
    }
}
