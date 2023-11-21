using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class RevalidarTokenServicoAcessosQueryHandler : IRequestHandler<RevalidarTokenServicoAcessosQuery, UsuarioPerfisRetornoDTO>
    {
        private readonly IMapper _mapper;

        private readonly IServicoAcessos _servicoAcessos;

        public RevalidarTokenServicoAcessosQueryHandler(IMapper mapper, IServicoAcessos servicoAcessos)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
        }

        public async Task<UsuarioPerfisRetornoDTO> Handle(RevalidarTokenServicoAcessosQuery request, CancellationToken cancellationToken)
        {
            var retornoPerfis = await _servicoAcessos.RevalidarToken(request.Token);
            return _mapper.Map<UsuarioPerfisRetornoDTO>(retornoPerfis);
        }
    }
}
