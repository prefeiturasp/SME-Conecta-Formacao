using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Constantes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuariosPorPerfilQueryHandler : IRequestHandler<ObterUsuariosPorPerfilQuery, IEnumerable<RetornoUsuarioLoginNomeDTO>>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public ObterUsuariosPorPerfilQueryHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<RetornoUsuarioLoginNomeDTO>> Handle(ObterUsuariosPorPerfilQuery request, CancellationToken cancellationToken)
        {
            var usuariosAdminDf = await _mediator.Send(new ObterUsuariosPorPerfisServicoEolQuery(request.Perfis), cancellationToken);
            return !usuariosAdminDf.Any()
                ? Enumerable.Empty<RetornoUsuarioLoginNomeDTO>()
                : _mapper.Map<IEnumerable<RetornoUsuarioLoginNomeDTO>>(usuariosAdminDf);
        }
    }
}