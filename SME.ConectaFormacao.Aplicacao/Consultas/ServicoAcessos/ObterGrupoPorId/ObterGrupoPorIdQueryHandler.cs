using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Grupo;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterGrupoPorIdQueryHandler : IRequestHandler<ObterGrupoPorIdQuery, GrupoDTO>
    {
        private readonly IMapper _mapper;
        private readonly IServicoAcessos _servicoAcessos;

        public ObterGrupoPorIdQueryHandler(IMapper mapper, IServicoAcessos servicoAcessos)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
        }

        public async Task<GrupoDTO> Handle(ObterGrupoPorIdQuery request, CancellationToken cancellationToken)
        {
            var grupo = await _servicoAcessos.ObterGrupoPorId(request.GrupoId);
            return _mapper.Map<GrupoDTO>(grupo);
        }
    }
}