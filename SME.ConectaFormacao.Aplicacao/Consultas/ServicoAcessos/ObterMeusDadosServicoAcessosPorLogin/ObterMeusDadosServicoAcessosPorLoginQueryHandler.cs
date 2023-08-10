using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterMeusDadosServicoAcessosPorLoginQueryHandler : IRequestHandler<ObterMeusDadosServicoAcessosPorLoginQuery, DadosUsuarioDTO>
    {
        private readonly IMapper _mapper;
        private readonly IServicoAcessos _servicoAcessos;

        public ObterMeusDadosServicoAcessosPorLoginQueryHandler(IMapper mapper, IServicoAcessos servicoAcessos)
        {
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
        }

        public async Task<DadosUsuarioDTO> Handle(ObterMeusDadosServicoAcessosPorLoginQuery request, CancellationToken cancellationToken)
        {
            var acessoDadosUsuario = await _servicoAcessos.ObterMeusDados(request.Login);
            return _mapper.Map<DadosUsuarioDTO>(acessoDadosUsuario);
        }
    }
}
