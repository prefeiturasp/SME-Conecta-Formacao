using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPerfisUsuarioServicoAcessosPorLoginQueryHandler : IRequestHandler<ObterPerfisUsuarioServicoAcessosPorLoginQuery, UsuarioPerfisRetornoDTO>
    {
        private readonly IMapper _mapper;
        private readonly IServicoAcessos _servicoAcessos;

        public ObterPerfisUsuarioServicoAcessosPorLoginQueryHandler(IMapper mapper, IServicoAcessos servicoAcessos)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
        }

        public async Task<UsuarioPerfisRetornoDTO> Handle(ObterPerfisUsuarioServicoAcessosPorLoginQuery request, CancellationToken cancellationToken)
        {
            var retornoPerfis = await _servicoAcessos.ObterPerfisUsuario(request.Login);
            return _mapper.Map<UsuarioPerfisRetornoDTO>(retornoPerfis);
        }
    }
}
